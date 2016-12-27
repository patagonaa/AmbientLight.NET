using System.Drawing;
using System.Drawing.Imaging;

namespace AmbientLightNet.ScreenCapture.Infrastructure
{
	public class NonCachedScreenRegionBitmapProvider : IScreenRegionBitmapProvider
	{
		public Bitmap ProvideForScreenRegion(ScreenRegion region, int width, int height, PixelFormat pixelFormat)
		{
			return new Bitmap(width, height, pixelFormat);
		}

		public void Dispose()
		{
		}
	}
}