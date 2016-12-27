using AmbientLightNet.DesktopDuplicationScreenCapture;
using AmbientLightNet.ScreenCapture.Infrastructure;

namespace AmbientLightNet.Infrastructure.ScreenCapture
{
	public class ScreenCaptureServiceProvider
	{
		public IScreenCaptureService Provide(bool useCache)
		{
			return new DesktopDuplicationScreenCaptureService(useCache);
		}
	}
}
