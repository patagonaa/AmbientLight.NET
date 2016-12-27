using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace AmbientLightNet.Infrastructure.ColorAveraging
{
	public class GdiFastPixelAveraging : IColorAveragingService
	{
		private readonly int _skipColumns;
		private readonly int _skipRows;
		private byte[] _data;

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

			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmapWidth, bitmapHeight), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

			int dataLength = bitmapData.Stride * bitmapData.Height;
			
			var i = 0;
			long rValues = 0;
			long gValues = 0;
			long bValues = 0;

			if (_data == null || _data.Length < dataLength)
			{
				_data = new byte[dataLength];
			}

			lock (_data)
			{
				Marshal.Copy(bitmapData.Scan0, _data, 0, dataLength);
				int stride = bitmapData.Stride;
				bitmap.UnlockBits(bitmapData);
				
				const int bytesPerPixel = 4;

				for (var column = 0; column < bitmapHeight; column += 1 + _skipColumns)
				{
					int columnPos = column * stride;
					for (var row = 0; row < bitmapWidth; row += 1 + _skipRows)
					{
						int rowPos = columnPos + row * bytesPerPixel;
						// var a = _data[rowPos + 3];
						rValues += _data[rowPos + 2];
						gValues += _data[rowPos + 1];
						bValues += _data[rowPos];
						
						i++;
					}
				}
			}

			return Color.FromArgb(255, (byte)(rValues/i), (byte) (gValues/i), (byte) (bValues/i));
		}

		public void Dispose()
		{
		}
	}
}