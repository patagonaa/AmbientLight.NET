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
		private readonly ColorTransformerProvider _colorTransformerProvider;
		private IList<ScreenRegion> _screenRegions; //needed as reference for caching
		private readonly ScreenCaptureServiceProvider _screenCaptureServiceProvider;

		public AmbiLightService(string configPath, ILogger logger)
		{
			_configPath = configPath;
			_logger = logger;

			_screenCaptureServiceProvider = new ScreenCaptureServiceProvider();
			_colorAveragingServiceProvider = new ColorAveragingServiceProvider();
			_colorTransformerProvider = new ColorTransformerProvider();

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

				foreach (var output in region.Outputs)
				{
					IOutputInfo outputInfo = output.OutputInfo;
					IOutputPlugin plugin = plugins.FirstOrDefault(y => y.Name == outputInfo.PluginName);
					if (plugin == null)
						throw new InvalidOperationException(string.Format("Missing OutputPlugin {0}", outputInfo.PluginName));

					OutputService outputService = plugin.GetNewOutputService();
					outputService.Initialize(outputInfo);

					var outputConfig = new OutputConfiguration
					{
						ColorTransformers = _colorTransformerProvider.Provide(output.ColorTransformerConfigs),
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
			const int resendMilliseconds = 500;

			const int minMillis = 1000 / maxFps;

			while (_running)
			{
				lock (_configLock)
				{
					DateTime startDt = DateTime.UtcNow;

					Task<IList<Bitmap>> captureTask = Task.Run(() => _screenCaptureService.CaptureScreenRegions(_screenRegions, true));
					//Task<IList<Bitmap>> captureTask = Task.FromResult(_screenCaptureService.CaptureScreenRegions(_screenRegions, true));

					Task outputTask = captureTask.ContinueWith(bitmapTask =>
					{
						IList<Bitmap> bitmaps = bitmapTask.Result;

						for (var i = 0; i < _regionConfigurations.Count; i++)
						{
							RegionConfiguration regionConfig = _regionConfigurations[i];

							Bitmap bitmap = bitmaps[i];

							Color colorToSet = bitmap == null
								? regionConfig.LastColor
								: regionConfig.ColorAveragingService.GetAverageColor(bitmap);

							if (regionConfig.LastColor != colorToSet)
							{
								_logger.Log(LogLevel.Info, "[{0}] Color changed. outputting!", regionConfig.Id);

								regionConfig.LastColor = colorToSet;
								regionConfig.LastColorSetTime = DateTime.UtcNow;
								OutputColor(regionConfig.OutputConfigs, colorToSet);
							}
						}

						DateTime endDt = DateTime.UtcNow;

						var timeSpan = (int) (endDt - startDt).TotalMilliseconds;

						if (timeSpan < minMillis)
						{
							int waitTime = minMillis - timeSpan;
							Thread.Sleep(waitTime);
						}
					});

					while (outputTask.Status != TaskStatus.RanToCompletion)
					{
						var utcNow = DateTime.UtcNow;

						foreach (var regionConfig in _regionConfigurations)
						{
							if (regionConfig.LastColorSetTime + TimeSpan.FromMilliseconds(resendMilliseconds) < utcNow)
							{
								_logger.Log(LogLevel.Info, "[{0}] Resend Timeout elapsed. outputting!", regionConfig.Id);
								regionConfig.LastColorSetTime = utcNow;
								OutputColor(regionConfig.OutputConfigs, regionConfig.LastColor);
							}
						}

						Thread.Sleep(50);
					}
				}
			}
		}

		private async void OutputColor(IEnumerable<OutputConfiguration> outputConfigs, Color averageColor)
		{
			var outputTasks = new List<Task>();

			foreach (OutputConfiguration outputConfig in outputConfigs)
			{
				OutputService outputService = outputConfig.OutputService;

				IList<IColorTransformer> colorTransformers = outputConfig.ColorTransformers;

				Color outputColor = colorTransformers.Aggregate(averageColor, (color, transformer) => transformer.Transform(color));

				outputTasks.Add(Task.Run(() =>
				{
					try
					{
						outputService.Output(outputColor);
					}
					catch (Exception ex)
					{
						_logger.Log(LogLevel.Error, "Output Color Failed: " + ex.ToString());
					}
				}));
			}

			await Task.WhenAll(outputTasks.ToArray());
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
				LastColor = Color.Black;
			}

			public int Id { get; private set; }
			public ScreenRegion ScreenRegion { get; set; }
			public IColorAveragingService ColorAveragingService { get; set; }
			public Color LastColor { get; set; }
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
			public IList<IColorTransformer> ColorTransformers { get; set; }
			public OutputService OutputService { get; set; }
			public void Dispose()
			{
				OutputService.Dispose();
			}
		}
	}
}