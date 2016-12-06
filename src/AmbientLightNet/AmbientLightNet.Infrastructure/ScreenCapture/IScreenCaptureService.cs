using System;
using System.Collections.Generic;
using System.Drawing;
using AmbientLightNet.Infrastructure.AmbiLightConfig;

namespace AmbientLightNet.Infrastructure.ScreenCapture
{
	public interface IScreenCaptureService : IDisposable
	{
		IList<Bitmap> CaptureScreenRegions(IList<ScreenRegion> regions, bool useCache = false);
		void ClearBitmapCache();
	}
}