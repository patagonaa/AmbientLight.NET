using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace AmbientLightNet.Infrastructure.ColorAveraging
{
	public class GdiFastPixelAveraging : IColorAveragingService
	{
		private readonly int _skipColumns;
		private readonly int _skipRows;

		public GdiFastPixelAveraging(IDictionary<string, object> config)
			: this(
				Convert.ToInt32(config["skipColumns"]),
				Convert.ToInt32(config["skipRows"]))
		{
		}

		public GdiFastPixelAveraging(int skipColumns = 0, int skipRows = 0)
		{
			_skipColumns = skipColumns;
			_skipRows = skipRows;
		}

		public Color GetAverageColor(Bitmap bitmap)
		{
			int bitmapWidth = bitmap.Width;
			int bitmapHeight = bitmap.Height;

			int bytesPerPixel;

			PixelFormat pixelFormat = bitmap.PixelFormat;
			switch (pixelFormat)
			{
				case PixelFormat.Format24bppRgb:
					bytesPerPixel = 3;
					break;
				case PixelFormat.Format32bppRgb:
				case PixelFormat.Format32bppArgb:
					bytesPerPixel = 4;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var i = 0;
			long rValues = 0;
			long gValues = 0;
			long bValues = 0;
			
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmapWidth, bitmapHeight), ImageLockMode.ReadOnly, pixelFormat);

			int stride = bitmapData.Stride;
			unsafe
			{
				var data = (byte*) bitmapData.Scan0;

				for (var y = 0; y < bitmapHeight; y += 1 + _skipColumns)
				{
					int lineIndex = y*stride;
					for (var x = 0; x < bitmapWidth; x += 1 + _skipRows)
					{
						int pixelIndex = lineIndex + x*bytesPerPixel;
						rValues += data[pixelIndex + 2];
						gValues += data[pixelIndex + 1];
						bValues += data[pixelIndex];

						i++;
					}
				}
			}

			bitmap.UnlockBits(bitmapData);

			return Color.FromArgb(255, (byte) (rValues/i), (byte) (gValues/i), (byte) (bValues/i));
		}

		public void Dispose()
		{
		}
	}
}