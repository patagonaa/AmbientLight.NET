using System.Drawing;

namespace AmbientLightNet.ScreenCapture.Infrastructure
{
	public class CaptureResult
	{
		public CaptureState State { get; set; }
		public Bitmap Bitmap { get; set; }
	}
}