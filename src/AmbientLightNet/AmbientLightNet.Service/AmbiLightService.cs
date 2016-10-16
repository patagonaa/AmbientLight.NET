using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using AmbientLightNet.Infrastructure;
using AmbiLightNet.PluginBase;
using Newtonsoft.Json;

namespace AmbientLightNet.Service
{
	public class AmbiLightService : IDisposable
	{
		private readonly string _configPath;
		private bool _running;
		private readonly IScreenCaptureService _screenCaptureService;
		private readonly IColorAveragingService _colorAveragingService;
		private readonly object _configLock = new object();
		private IList<ScreenRegion> _regions;
		private List<OutputService> _outputServices;

		public AmbiLightService(string configPath)
		{
			_configPath = configPath;
			_screenCaptureService = new GdiScreenCaptureService();

			//_colorAveragingService = new GdiDownScalingAveraging(10, 10, InterpolationMode.NearestNeighbor);
			_colorAveragingService = new GdiFastPixelAveraging(10, 10);

			AmbiLightConfig config = ReadConfig(_configPath);

			ApplyConfig(config);
		}

		private void ApplyConfig(AmbiLightConfig config)
		{
			List<IOutputPlugin> plugins = OutputPlugins.GetAvailablePlugins();

			_regions = config.RegionsToOutput.Select(x => x.ScreenRegion).ToList();

			if (_outputServices != null)
			{
				foreach (OutputService outputService in _outputServices)
				{
					outputService.Dispose();
				}
			}
			_outputServices = config.RegionsToOutput
				.Select(x =>
				{
					IOutputPlugin plugin = plugins.First(y => y.Name == x.OutputInfo.PluginName);
					OutputService outputService = plugin.GetNewOutputService();
					outputService.Initialize(x.OutputInfo);
					return outputService;
				})
				.ToList();
		}

		public void Start()
		{
			_running = true;

			const int fps = 60;
			const int millis = 1000 / fps;

			while (_running)
			{
				lock (_configLock)
				{
					DateTime startDt = DateTime.UtcNow;

					IList<Bitmap> bitmaps = _screenCaptureService.CaptureScreenRegions(_regions, true);

					for (var i = 0; i < _regions.Count; i++)
					{
						OutputService outputService = _outputServices[i];
						Bitmap bitmap = bitmaps[i];

						Color averageColor = _colorAveragingService.GetAverageColor(bitmap);

						outputService.Output(averageColor);
					}

					DateTime endDt = DateTime.UtcNow;

					var timeSpan = (int)(endDt - startDt).TotalMilliseconds;
					if (timeSpan >= millis)
					{
						Console.WriteLine("Could not keep up! {0}ms too slow", timeSpan - millis);
						continue;
					}
					Thread.Sleep(millis - timeSpan);
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
			return JsonConvert.DeserializeObject<AmbiLightConfig>(File.ReadAllText(fileName, Encoding.UTF8));
		}

		public void Stop()
		{
			_running = false;
		}

		public void Dispose()
		{
			_screenCaptureService.Dispose();
			_colorAveragingService.Dispose();
		}
	}
}