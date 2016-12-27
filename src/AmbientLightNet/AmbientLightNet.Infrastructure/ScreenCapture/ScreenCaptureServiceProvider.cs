using AmbientLightNet.GdiScreenCapture;
using AmbientLightNet.ScreenCapture.Infrastructure;

namespace AmbientLightNet.Infrastructure.ScreenCapture
{
	public class ScreenCaptureServiceProvider
	{
		public IScreenCaptureService Provide()
		{
			return new GdiScreenCaptureService();
		}
	}
}
