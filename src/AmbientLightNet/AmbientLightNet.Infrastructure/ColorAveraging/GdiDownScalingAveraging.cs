using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using AmbiLightNet.PluginBase;

namespace AmbientLightNet.Infrastructure.ColorAveraging
{
	public class GdiDownScalingAveraging : IColorAveragingService
	{
		private readonly int _bitmapWidth;
		private readonly int _bitmapHeight;
		private readonly Bitmap _bitmap;
		private readonly Graphics _graphics;

		public GdiDownScalingAveraging(IDictionary<string, object> config)
			: this(
				Convert.ToInt32(config["width"]),
				Convert.ToInt32(config["height"]),
				(InterpolationMode) config["interpolationMode"])
		{
		}

		public GdiDownScalingAveraging(int width, int height, InterpolationMode interpolationMode)
		{
			_bitmapWidth = width;
			_bitmapHeight = height;
			_bitmap = new Bitmap(width, height);
			_graphics = Graphics.FromImage(_bitmap);
			_graphics.CompositingMode = CompositingMode.SourceCopy;
			_graphics.InterpolationMode = interpolationMode;
		}

		public ColorF GetAverageColor(Bitmap bitmap)
		{
			_graphics.DrawImage(bitmap, new Rectangle(0, 0, _bitmapWidth, _bitmapHeight), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel);

			var colors = new List<Color>();
			for (var y = 0; y < _bitmapHeight; y++)
			{
				for (var x = 0; x < _bitmapWidth; x++)
				{
					colors.Add(_bitmap.GetPixel(x, y));
				}
			}

			var averageR = (float) colors.Average(x => x.R);
			var averageG = (float) colors.Average(x => x.G);
			var averageB = (float) colors.Average(x => x.B);

			return ColorF.FromRgb(averageR/255f, averageG/255f, averageB/255f);
		}

		public void Dispose()
		{
			_bitmap.Dispose();
			_graphics.Dispose();
		}
	}
}