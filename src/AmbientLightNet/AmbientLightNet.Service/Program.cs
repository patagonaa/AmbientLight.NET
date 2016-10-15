using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

			var config = JsonConvert.DeserializeObject<AmbiLightConfig>(File.ReadAllText(args[0], Encoding.UTF8));

			List<IOutputPlugin> plugins = OutputPlugins.GetAvailablePlugins();

			IScreenCaptureService screenCaptureService = new GdiScreenCaptureService();

			List<ScreenRegion> regions = config.RegionsToOutput.Select(x => x.ScreenRegion).ToList();

			List<OutputService> outputServices = config.RegionsToOutput
				.Select(x =>
				{
					IOutputPlugin plugin = plugins.First(y => y.Name == x.OutputInfo.PluginName);
					OutputService outputService = plugin.GetNewOutputService();
					outputService.Initialize(x.OutputInfo);
					return outputService;
				})
				.ToList();

			const int fps = 20;
			const int millis = 1000/fps;

			while (true)
			{
				//TODO: file watcher on config

				DateTime startDt = DateTime.UtcNow;

				IList<Bitmap> bitmaps = screenCaptureService.CaptureScreenRegions(regions);

				for (var i = 0; i < regions.Count; i++)
				{
					OutputService outputService = outputServices[i];
					Bitmap bitmap = bitmaps[i];

					using (var tmpBitmap = new Bitmap(1, 1))
					{
						using (Graphics tmpGraphics = Graphics.FromImage(tmpBitmap))
						{
							tmpGraphics.DrawImage(bitmap, new Rectangle(Point.Empty, new Size(1, 1)), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel);
						}
						Color color = tmpBitmap.GetPixel(0, 0);
						outputService.Output(color.R/255d, color.G/255d, color.B/255d);
					}
				}

				foreach (Bitmap bitmap in bitmaps)
				{
					bitmap.Dispose();
				}

				DateTime endDt = DateTime.UtcNow;

				TimeSpan timeSpan = (endDt - startDt);
				if ((int) timeSpan.TotalMilliseconds >= millis)
				{
					Console.WriteLine("Could not keep up! {0}ms too slow", timeSpan.TotalMilliseconds - millis);
					continue;
				}
				Thread.Sleep(millis - (int)timeSpan.TotalMilliseconds);
			}
		}
	}
}
