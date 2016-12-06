using System.Drawing;

namespace AmbientLightNet.Infrastructure.AmbiLightConfig
{
	public class ScreenRegion
	{
		public string ScreenName { get; set; }
		public RectangleF Rectangle { get; set; }

		public override string ToString()
		{
			return string.Format("{0}: {1}", ScreenName, Rectangle);
		}
	}
}