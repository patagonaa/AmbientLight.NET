using System;
using System.Collections.Generic;

namespace AmbientLightNet.ScreenCapture.Infrastructure
{
	public interface IScreenCaptureService : IDisposable
	{
		IList<CaptureResult> CaptureScreenRegions(IList<ScreenRegion> regions, bool mayBlockIfNoChanges);
	}
}