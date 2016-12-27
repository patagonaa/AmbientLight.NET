using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace AmbientLightNet.ScreenCapture.Infrastructure
{
	public interface IScreenRegionBitmapProvider : IDisposable
	{
		Bitmap ProvideForScreenRegion(ScreenRegion region, int width, int height, PixelFormat pixelFormat);
	}
}
