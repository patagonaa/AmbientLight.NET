using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AmbientLightNet.Infrastructure;
using AmbientLightNet.Infrastructure.AmbiLightConfig;
using AmbientLightNet.Infrastructure.ColorAveraging;
using AmbientLightNet.Infrastructure.ColorTransformer;
using AmbientLightNet.Infrastructure.Logging;
using AmbientLightNet.Infrastructure.ScreenCapture;
using AmbientLightNet.ScreenCapture.Infrastructure;
using AmbiLightNet.PluginBase;
using Newtonsoft.Json;

namespace AmbientLightNet.Service
{
	public class AmbiLightService : IDisposable
	{
		private readonly string _configPath;
		private readonly ILogger _logger;
		private bool _running;
		private IScreenCaptureService _screenCaptureService;
		private readonly object _configLock = new object();
		private IList<RegionConfiguration> _regionConfigurations;
		private readonly ColorAveragingServiceProvider _colorAveragingServiceProvider;
		private readonly ColorTransformerService _colorTransformerService;
		private IList<ScreenRegion> _screenRegions; //needed as reference for caching
		private readonly ScreenCaptureServiceProvider _screenCaptureServiceProvider;

		public AmbiLightService(string configPath, ILogger logger)
		{
			_configPath = configPath;
			_logger = logger;

			_screenCaptureServiceProvider = new ScreenCaptureServiceProvider();
			_colorAveragingServiceProvider = new ColorAveragingServiceProvider();
			_colorTransformerService = new ColorTransformerService();

			AmbiLightConfig config = ReadConfig(_configPath);

			ApplyConfig(config);
		}

		private void ApplyConfig(AmbiLightConfig config)
		{
			if(_screenCaptureService != null)
				_screenCaptureService.Dispose();

			_screenCaptureService = _screenCaptureServiceProvider.Provide(true);


			List<IOutputPlugin> plugins = OutputPlugins.GetAvailablePlugins();

			List<ScreenRegionOutput> regions = config.RegionsToOutput;

			if (_regionConfigurations != null)
			{
				foreach (RegionConfiguration regionConfiguration in _regionConfigurations)
				{
					regionConfiguration.Dispose();
				}
			}

			_regionConfigurations = new List<RegionConfiguration>();

			var regionConfigId = 0;

			foreach (ScreenRegionOutput region in regions)
			{
				var regionConfig = new RegionConfiguration(regionConfigId++)
				{
					ScreenRegion = region.ScreenRegion,
					ColorAveragingService = _colorAveragingServiceProvider.Provide(region.ColorAveragingConfig),
					OutputConfigs = new List<OutputConfiguration>()
				};

				var outputId = 0;

				foreach (Output output in region.Outputs)
				{
					IOutputInfo outputInfo = output.OutputInfo;
					IOutputPlugin plugin = plugins.FirstOrDefault(y => y.Name == outputInfo.PluginName);
					if (plugin == null)
						throw new InvalidOperationException(string.Format("Missing OutputPlugin {0}", outputInfo.PluginName));

					OutputService outputService = plugin.GetNewOutputService();
					outputService.Initialize(outputInfo);

					IList<ColorTransformerConfig> colorTransformerConfigs = output.ColorTransformerConfigs;

					if (colorTransformerConfigs == null)
					{
						colorTransformerConfigs = new List<ColorTransformerConfig>
						{
							new ColorTransformerConfig
							{
								Type = typeof (HysteresisColorTransformer),
								Config = new Dictionary<string, object>
								{
									{"hysteresis", "0.01"}
								}
							},
							new ColorTransformerConfig
							{
								Type = typeof (BrightnessColorTransformer),
								Config = new Dictionary<string, object>
								{
									{"factorR", "1"},
									{"factorG", "0.9"},
									{"factorB", "0.4"}
								}
							},
							new ColorTransformerConfig
							{
								Type = typeof (GammaColorTransformer),
								Config = new Dictionary<string, object>
								{
									{"gammaR", "1"},
									{"gammaG", "1.2"},
									{"gammaB", "1.2"}
								}
							},
							new ColorTransformerConfig
							{
								Type = typeof (ThresholdColorTransformer),
								Config = new Dictionary<string, object>
								{
									{"threshold", "0.02"}
								}
							}
						};
					}

					var outputConfig = new OutputConfiguration(regionConfig.Id, outputId++)
					{
						ColorTransformerContexts = colorTransformerConfigs.Select(x => new ColorTransformerContext(x)).ToList(),
						OutputService = outputService,
						ResendInterval = outputService.DefaultResendInterval
					};

					regionConfig.OutputConfigs.Add(outputConfig);
				}

				_regionConfigurations.Add(regionConfig);
			}

			_screenRegions = _regionConfigurations.Select(x => x.ScreenRegion).ToList();
		}

		public void Start()
		{
			_running = true;
			
			const int maxFps = 60;

			const int minMillis = 1000 / maxFps;

			while (_running)
			{
				lock (_configLock)
				{
					DateTime startDt = DateTime.UtcNow;

					Task<IList<CaptureResult>> captureTask = Task.Run(() => _screenCaptureService.CaptureScreenRegions(_screenRegions, true));
					//Task<IList<CaptureResult>> captureTask = Task.FromResult(_screenCaptureService.CaptureScreenRegions(_screenRegions, true));

					Task outputAllTask = captureTask.ContinueWith(HandleCaptureResults);

					while (outputAllTask.Status != TaskStatus.RanToCompletion)
					{
						if (outputAllTask.Exception != null)
							throw outputAllTask.Exception;

						Thread.Sleep(10);

						var utcNow = DateTime.UtcNow;

						var outputTasks = new List<Task>();

						foreach (RegionConfiguration regionConfig in _regionConfigurations)
						{
							foreach (OutputConfiguration outputConfig in regionConfig.OutputConfigs)
							{
								if (outputConfig.ResendInterval != null && outputConfig.LastColorSetTime + outputConfig.ResendInterval.Value < utcNow)
								{
									outputConfig.LastColorSetTime = utcNow;
									Task outputTask = OutputColor(outputConfig, regionConfig.LastColor, true);
									_logger.Log(LogLevel.Debug, "[Region {0} Output {1}] Resend timeout elapsed.", outputConfig.RegionId, outputConfig.OutputId);
									outputTasks.Add(outputTask);
								}
							}
						}

						Task.WhenAll(outputTasks).Wait();
					}

					DateTime endDt = DateTime.UtcNow;

					var timeSpan = (int)(endDt - startDt).TotalMilliseconds;

					if (timeSpan < minMillis)
					{
						int waitTime = minMillis - timeSpan;
						Thread.Sleep(waitTime);
					}
				}
			}
		}

		private void HandleCaptureResults(Task<IList<CaptureResult>> captureTask)
		{
			IList<CaptureResult> captureResults = captureTask.Result;

			var outputTasks = new List<Task>();

			for (var i = 0; i < _regionConfigurations.Count; i++)
			{
				RegionConfiguration regionConfig = _regionConfigurations[i];

				CaptureResult captureResult = captureResults[i];

				ColorF colorToSet;

				switch (captureResult.State)
				{
					case CaptureState.NoChanges:
						colorToSet = regionConfig.LastColor;
						break;
					case CaptureState.NewBitmap:
						colorToSet = regionConfig.ColorAveragingService.GetAverageColor(captureResult.Bitmap);
						break;
					case CaptureState.ScreenNotFound:
						colorToSet = (ColorF) Color.Black;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				regionConfig.LastColor = colorToSet;

				_logger.Log(LogLevel.Debug, "[Region {0}] New frame available.", regionConfig.Id);

				foreach (OutputConfiguration outputConfig in regionConfig.OutputConfigs)
				{
					Task outputTask = OutputColor(outputConfig, colorToSet, false);
					if (outputTask != null)
					{
						_logger.Log(LogLevel.Debug, "[Region {0} Output {1}] Color changed. outputting", outputConfig.RegionId, outputConfig.OutputId);
						outputTasks.Add(outputTask);
					}
				}
			}

			Task.WhenAll(outputTasks).Wait();
		}

		private Task OutputColor(OutputConfiguration outputConfig, ColorF colorToSet, bool resendSameColor)
		{
			OutputService outputService = outputConfig.OutputService;

			IList<ColorTransformerContext> colorTransformerContexts = outputConfig.ColorTransformerContexts;

			var outputColor = _colorTransformerService.Transform(colorTransformerContexts, colorToSet);

			if (!resendSameColor && outputConfig.LastColor.HasValue && outputService.ColorsEqual(outputColor, outputConfig.LastColor.Value))
				return null;

			outputConfig.LastColor = outputColor;
			outputConfig.LastColorSetTime = DateTime.UtcNow;
			return Task.Run(() =>
			{
				try
				{
					outputService.Output(outputColor);
				}
				catch (Exception ex)
				{
					_logger.Log(LogLevel.Error, "[Region {0} Output {1}] Output Color Failed: {2}", outputConfig.RegionId, outputConfig.OutputId, ex.ToString());
				}
			});
		}

		public void ReloadConfig()
		{
			lock (_configLock)
			{
				_logger.Log(LogLevel.Info, "Reloading config...");
				AmbiLightConfig config = ReadConfig(_configPath);
				ApplyConfig(config);
			}
		}

		private static AmbiLightConfig ReadConfig(string fileName)
		{
			return JsonConvert.DeserializeObject<AmbiLightConfig>(File.ReadAllText(fileName, Encoding.UTF8), new JsonSerializerSettings {MissingMemberHandling = MissingMemberHandling.Ignore});
		}

		public void Stop()
		{
			_running = false;
		}

		public void Dispose()
		{
			_screenCaptureService.Dispose();

			foreach (var regionConfiguration in _regionConfigurations)
			{
				regionConfiguration.Dispose();
			}
		}

		private class RegionConfiguration : IDisposable
		{
			public RegionConfiguration(int id)
			{
				Id = id;
				LastColor = (ColorF) Color.Black;
			}

			public int Id { get; private set; }
			public ScreenRegion ScreenRegion { get; set; }
			public IColorAveragingService ColorAveragingService { get; set; }
			public IList<OutputConfiguration> OutputConfigs { get; set; }
			public ColorF LastColor { get; set; }

			public void Dispose()
			{
				ColorAveragingService.Dispose();
				foreach (var outputConfig in OutputConfigs)
				{
					outputConfig.Dispose();
				}
			}
		}

		private class OutputConfiguration : IDisposable
		{
			public OutputConfiguration(int regionId, int outputId)
			{
				RegionId = regionId;
				OutputId = outputId;
			}

			public int RegionId { get; private set; }
			public int OutputId { get; private set; }
			public OutputService OutputService { get; set; }
			public IList<ColorTransformerContext> ColorTransformerContexts { get; set; }
			public ColorF? LastColor { get; set; }
			public DateTime LastColorSetTime { get; set; }
			public TimeSpan? ResendInterval { get; set; }

			public void Dispose()
			{
				OutputService.Dispose();
			}
		}
	}
}