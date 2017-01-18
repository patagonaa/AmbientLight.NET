using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using AmbientLightNet.ScreenCapture.Infrastructure;

namespace AmbientLightNet.GdiScreenCapture
{
	public class GdiScreenCaptureService : IScreenCaptureService
	{
		private readonly IScreenRegionBitmapProvider _bitmapProvider;

		public GdiScreenCaptureService(bool useCache)
		{
			if (useCache)
				_bitmapProvider = new CachedScreenRegionBitmapProvider();
			else
				_bitmapProvider = new NonCachedScreenRegionBitmapProvider();
		}
		
		public IList<CaptureResult> CaptureScreenRegions(IList<ScreenRegion> regions, int? maxBlockMilliseconds)
		{
			Screen[] allScreens = Screen.AllScreens;
			var toReturn = new List<CaptureResult>();

			for (var i = 0; i < regions.Count; i++)
			{
				ScreenRegion region = regions[i];
				Screen screen = allScreens.SingleOrDefault(x => x.DeviceName == region.ScreenName);

				if (screen == null)
				{
					toReturn.Add(new CaptureResult {State = CaptureState.ScreenNotFound});
					continue;
				}

				var width = (int) (screen.Bounds.Width*region.Rectangle.Width);
				var height = (int) (screen.Bounds.Height*region.Rectangle.Height);
				var bitmap = _bitmapProvider.ProvideForScreenRegion(region, width, height, PixelFormat.Format24bppRgb);

				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					var positionX = (int) (screen.Bounds.X + (screen.Bounds.Width*region.Rectangle.X));
					var positionY = (int) (screen.Bounds.Y + (screen.Bounds.Height*region.Rectangle.Y));

					graphics.CopyFromScreen(positionX, positionY, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
				}

				toReturn.Add(new CaptureResult {State = CaptureState.NewBitmap, Bitmap = bitmap});
			}

			return toReturn;
		}

		public void Dispose()
		{
			_bitmapProvider.Dispose();
		}
	}
}