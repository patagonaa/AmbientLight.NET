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
using AmbientLightNet.Infrastructure.ScreenCapture;
using AmbiLightNet.PluginBase;
using Newtonsoft.Json;

namespace AmbientLightNet.Service
{
	public class AmbiLightService : IDisposable
	{
		private readonly string _configPath;
		private bool _running;
		private readonly IScreenCaptureService _screenCaptureService;
		private readonly object _configLock = new object();
		private IList<RegionConfiguration> _regionConfigurations;
		private readonly ColorAveragingServiceProvider _colorAveragingServiceProvider;
		private readonly ColorTransformerProvider _colorTransformerProvider;
		private IList<ScreenRegion> _screenRegions; //needed as reference for caching

		public AmbiLightService(string configPath)
		{
			_configPath = configPath;
			_screenCaptureService = new GdiScreenCaptureService();

			_colorAveragingServiceProvider = new ColorAveragingServiceProvider();
			_colorTransformerProvider = new ColorTransformerProvider();

			AmbiLightConfig config = ReadConfig(_configPath);

			ApplyConfig(config);
		}

		private void ApplyConfig(AmbiLightConfig config)
		{
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

			foreach (ScreenRegionOutput region in regions)
			{
				var regionConfig = new RegionConfiguration
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

			const int numSamples = 10;
			const int maxFps = 30;

			const int minMillis = 1000 / maxFps;
			var timeSamples = new Queue<int>(numSamples);

			var outputTasks = new List<Task>();

			while (_running)
			{
				lock (_configLock)
				{
					DateTime startDt = DateTime.UtcNow;

					IList<Bitmap> bitmaps = _screenCaptureService.CaptureScreenRegions(_screenRegions, true);

					for (var i = 0; i < _regionConfigurations.Count; i++)
					{
						var regionConfiguration = _regionConfigurations[i];

						Bitmap bitmap = bitmaps[i];

						Color averageColor = regionConfiguration.ColorAveragingService.GetAverageColor(bitmap);

						Task.WhenAll(outputTasks.ToArray()).Wait();
						outputTasks.Clear();

						foreach (var outputConfig in regionConfiguration.OutputConfigs)
						{
							Color outputColor = outputConfig.ColorTransformers.Aggregate(averageColor, (color, transformer) => transformer.Transform(color));

							var utcNow = DateTime.UtcNow;

							if (outputConfig.LastColor != outputColor || outputConfig.LastColorSetTime + TimeSpan.FromMilliseconds(200) > utcNow)
							{
								outputTasks.Add(Task.Run(() =>
								{
									try
									{
										outputConfig.OutputService.Output(outputColor);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Output Color Failed: " + ex.ToString());
									}
								}));

								outputConfig.LastColor = outputColor;
								outputConfig.LastColorSetTime = utcNow;
							}
						}
					}

					DateTime endDt = DateTime.UtcNow;

					var timeSpan = (int)(endDt - startDt).TotalMilliseconds;

					if (timeSpan < minMillis)
					{
						int waitTime = minMillis - timeSpan;
						Thread.Sleep(waitTime);
						timeSpan += waitTime;
					}

					if (timeSamples.Count == numSamples)
					{
						Console.WriteLine("{0:N0} fps", 1000 / timeSamples.Average());
						timeSamples.Dequeue();
					}

					timeSamples.Enqueue(timeSpan);
				}
			}
		}

		public void ReloadConfig()
		{
			lock (_configLock)
			{
				Console.WriteLine("Reloading config...");
				AmbiLightConfig config = ReadConfig(_configPath);
				ApplyConfig(config);
				_screenCaptureService.ClearBitmapCache();
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
			public ScreenRegion ScreenRegion { get; set; }
			public IColorAveragingService ColorAveragingService { get; set; }
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
			public Color? LastColor { get; set; }
			public DateTime LastColorSetTime { get; set; }

			public void Dispose()
			{
				OutputService.Dispose();
			}
		}
	}
}