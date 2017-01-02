using System;
using System.Collections.Generic;
using System.Drawing;

namespace AmbientLightNet.ScreenCapture.Infrastructure
{
	public interface IScreenCaptureService : IDisposable
	{
		IList<Bitmap> CaptureScreenRegions(IList<ScreenRegion> regions, bool mayBlockIfNoChanges);
	}
}