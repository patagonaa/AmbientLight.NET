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
						OutputService = outputService
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
			const int resendMilliseconds = 100;

			const int minMillis = 1000 / maxFps;

			while (_running)
			{
				lock (_configLock)
				{
					DateTime startDt = DateTime.UtcNow;

					Task<IList<Bitmap>> captureTask = Task.Run(() => _screenCaptureService.CaptureScreenRegions(_screenRegions, true));
					//Task<IList<Bitmap>> captureTask = Task.FromResult(_screenCaptureService.CaptureScreenRegions(_screenRegions, true));

					Task outputTask = captureTask.ContinueWith(HandleBitmaps);

					while (outputTask.Status != TaskStatus.RanToCompletion)
					{
						Thread.Sleep(50);

						var utcNow = DateTime.UtcNow;

						foreach (var regionConfig in _regionConfigurations)
						{
							if (regionConfig.LastColorSetTime + TimeSpan.FromMilliseconds(resendMilliseconds) < utcNow)
							{
								_logger.Log(LogLevel.Debug, "[Region {0}] Resend timeout elapsed.", regionConfig.Id);
								regionConfig.LastColorSetTime = utcNow;
								OutputColor(regionConfig.OutputConfigs, regionConfig.LastColor);
							}
						}
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

		private void HandleBitmaps(Task<IList<Bitmap>> bitmapTask)
		{
			IList<Bitmap> bitmaps = bitmapTask.Result;

			for (var i = 0; i < _regionConfigurations.Count; i++)
			{
				RegionConfiguration regionConfig = _regionConfigurations[i];

				Bitmap bitmap = bitmaps[i];

				ColorF colorToSet = bitmap == null
					? regionConfig.LastColor
					: regionConfig.ColorAveragingService.GetAverageColor(bitmap);

				regionConfig.LastColor = colorToSet;
				regionConfig.LastColorSetTime = DateTime.UtcNow;

				_logger.Log(LogLevel.Debug, "[Region {0}] New frame available.", regionConfig.Id);
				OutputColor(regionConfig.OutputConfigs, colorToSet);
			}
		}

		private void OutputColor(IEnumerable<OutputConfiguration> outputConfigs, ColorF colorToSet)
		{
			var outputTasks = new List<Task>();

			foreach (OutputConfiguration outputConfig in outputConfigs)
			{
				OutputService outputService = outputConfig.OutputService;

				IList<ColorTransformerContext> colorTransformerContexts = outputConfig.ColorTransformerContexts;

				var outputColor = _colorTransformerService.Transform(colorTransformerContexts, colorToSet);

				if (!outputService.ColorsEqual(outputColor, outputConfig.LastColor))
				{
					_logger.Log(LogLevel.Debug, "[Region {0} Output {1}] Color changed. Outputting!", outputConfig.RegionId, outputConfig.OutputId);

					outputTasks.Add(Task.Run(() =>
					{
						try
						{
							outputService.Output(outputColor);
						}
						catch (Exception ex)
						{
							_logger.Log(LogLevel.Error, "[Region {0} Output {1}] Output Color Failed: {2}", outputConfig.RegionId, outputConfig.OutputId, ex.ToString());
						}
					}));
					outputConfig.LastColor = outputColor;
				}
			}

			Task.WhenAll(outputTasks.ToArray()).Wait();
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
			return JsonConvert.DeserializeObject<AmbiLightConfig>(File.ReadAllText(fileName, Encoding.UTF8), new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Ignore });
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
			public ColorF LastColor { get; set; }
			public DateTime LastColorSetTime { get; set; }
			public IList<OutputConfiguration> OutputConfigs { get; set; }

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
				LastColor = (ColorF) Color.Black;
			}

			public int RegionId { get; private set; }
			public int OutputId { get; private set; }
			public OutputService OutputService { get; set; }
			public IList<ColorTransformerContext> ColorTransformerContexts { get; set; }
			public ColorF LastColor { get; set; }

			public void Dispose()
			{
				OutputService.Dispose();
			}
		}
	}
}