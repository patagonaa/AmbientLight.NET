using System.Drawing;

namespace AmbientLightNet.ScreenCapture.Infrastructure
{
	public class ScreenRegion
	{
		public ScreenRegion(string screenName, RectangleF rectangle)
		{
			ScreenName = screenName;
			Rectangle = rectangle;
		}

		public string ScreenName { get; private set; }
		public RectangleF Rectangle { get; private set; }

		public override string ToString()
		{
			return string.Format("{0}: {1}", ScreenName, Rectangle);
		}
	}
}