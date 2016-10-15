using System;
using System.Collections.Generic;
using System.Drawing;

namespace AmbientLightNet.Infrastructure
{
	public interface IScreenCaptureService : IDisposable
	{
		IList<Bitmap> CaptureScreenRegions(IList<ScreenRegion> regions);
	}
}