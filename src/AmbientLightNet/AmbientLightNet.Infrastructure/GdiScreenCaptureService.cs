using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AmbientLightNet.Infrastructure
{
	public class GdiScreenCaptureService : IScreenCaptureService
	{
		public IList<Bitmap> CaptureScreenRegions(IList<ScreenRegion> regions)
		{
			Screen[] allScreens = Screen.AllScreens;
			var bitmaps = new List<Bitmap>();

			foreach (ScreenRegion region in regions)
			{
				Screen screen = allScreens.Single(x => x.DeviceName == region.ScreenName);

				var positionX = (int) (screen.Bounds.X + (screen.Bounds.Width*region.Rectangle.X));
				var positionY = (int) (screen.Bounds.Y + (screen.Bounds.Height*region.Rectangle.Y));

				var width = (int) (screen.Bounds.Width*region.Rectangle.Width);
				var height = (int) (screen.Bounds.Height*region.Rectangle.Height);

				var bitmap = new Bitmap(width, height);

				using (Graphics g = Graphics.FromImage(bitmap))
				{
					g.CopyFromScreen(positionX, positionY, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
				}

				bitmaps.Add(bitmap);
			}

			return bitmaps; //TODO: cache bitmaps (and graphics?)
		}

		public void Dispose()
		{
		}
	}
}