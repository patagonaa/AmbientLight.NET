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
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length < 1 || !File.Exists(args[0]))
			{
				Console.WriteLine("Config Parameter with existing config expected!");
				return;
			}

			IScreenCaptureService screenCaptureService = new GdiScreenCaptureService();
			//IColorAveragingService colorAveragingService = new GdiDownScalingAveraging(10, 10, InterpolationMode.NearestNeighbor);
			IColorAveragingService colorAveragingService = new GdiFastPixelAveraging(50, 50);

			AmbiLightConfig config = ReadConfig(args[0]);
			
			screenCaptureService.ClearBitmapCache();

			List<IOutputPlugin> plugins = OutputPlugins.GetAvailablePlugins();

			IList<ScreenRegion> regions = config.RegionsToOutput.Select(x => x.ScreenRegion).ToList();

			List<OutputService> outputServices = config.RegionsToOutput
				.Select(x =>
				{
					IOutputPlugin plugin = plugins.First(y => y.Name == x.OutputInfo.PluginName);
					OutputService outputService = plugin.GetNewOutputService();
					outputService.Initialize(x.OutputInfo);
					return outputService;
				})
				.ToList();

			const int fps = 30;
			const int millis = 1000/fps;

			while (true)
			{
				//TODO: file watcher on config

				DateTime startDt = DateTime.UtcNow;

				IList<Bitmap> bitmaps = screenCaptureService.CaptureScreenRegions(regions, true);

				for (var i = 0; i < regions.Count; i++)
				{
					OutputService outputService = outputServices[i];
					Bitmap bitmap = bitmaps[i];

					Color averageColor = colorAveragingService.GetAverageColor(bitmap);

					outputService.Output(averageColor);
				}

				DateTime endDt = DateTime.UtcNow;

				var timeSpan = (int) (endDt - startDt).TotalMilliseconds;
				if (timeSpan >= millis)
				{
					Console.WriteLine("Could not keep up! {0}ms too slow", timeSpan - millis);
					continue;
				}
				Thread.Sleep(millis - timeSpan);
			}
		}

		private static AmbiLightConfig ReadConfig(string fileName)
		{
			var config = JsonConvert.DeserializeObject<AmbiLightConfig>(File.ReadAllText(fileName, Encoding.UTF8));
			return config;
		}
	}
}
