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

		private readonly Bitmap _blackBitmap;

		public GdiScreenCaptureService(bool useCache)
		{
			if (useCache)
				_bitmapProvider = new CachedScreenRegionBitmapProvider();
			else
				_bitmapProvider = new NonCachedScreenRegionBitmapProvider();

			_blackBitmap = new Bitmap(1, 1);
			using (Graphics graphics = Graphics.FromImage(_blackBitmap))
			{
				graphics.FillRectangle(Brushes.Black, 0, 0, 1, 1);
			}
		}
		
		public IList<Bitmap> CaptureScreenRegions(IList<ScreenRegion> regions, bool mayBlockIfNoChanges)
		{
			Screen[] allScreens = Screen.AllScreens;
			var bitmaps = new List<Bitmap>();
			
			for (var i = 0; i < regions.Count; i++)
			{
				ScreenRegion region = regions[i];
				Screen screen = allScreens.SingleOrDefault(x => x.DeviceName == region.ScreenName);

				Bitmap bitmap;

				if (screen != null)
				{
					var width = (int) (screen.Bounds.Width*region.Rectangle.Width);
					var height = (int) (screen.Bounds.Height*region.Rectangle.Height);
					bitmap = _bitmapProvider.ProvideForScreenRegion(region, width, height, PixelFormat.Format24bppRgb);

					using (Graphics graphics = Graphics.FromImage(bitmap))
					{
						var positionX = (int) (screen.Bounds.X + (screen.Bounds.Width*region.Rectangle.X));
						var positionY = (int) (screen.Bounds.Y + (screen.Bounds.Height*region.Rectangle.Y));

						graphics.CopyFromScreen(positionX, positionY, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
					}
				}
				else
				{
					bitmap = _blackBitmap; //TODO: this will not work if cache is off
				}

				bitmaps.Add(bitmap);
			}

			return bitmaps;
		}

		public void Dispose()
		{
			_bitmapProvider.Dispose();
			_blackBitmap.Dispose();
		}
	}
}