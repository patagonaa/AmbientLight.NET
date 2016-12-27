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
			_bitmapProvider = useCache
				? (IScreenRegionBitmapProvider) new CachedScreenRegionBitmapProvider()
				: (IScreenRegionBitmapProvider) new NonCachedScreenRegionBitmapProvider();

			_blackBitmap = new Bitmap(1, 1);
			using (Graphics graphics = Graphics.FromImage(_blackBitmap))
			{
				graphics.FillRectangle(Brushes.Black, 0, 0, 1, 1);
			}
		}
		
		public IList<Bitmap> CaptureScreenRegions(IList<ScreenRegion> regions)
		{
			Screen[] allScreens = Screen.AllScreens;
			var bitmaps = new List<Bitmap>();
			
			for (var i = 0; i < regions.Count; i++)
			{
				ScreenRegion region = regions[i];
				Screen screen = allScreens.SingleOrDefault(x => x.DeviceName == region.ScreenName);

				int width;
				int height;

				Bitmap bitmap;
				if (screen != null) // if screen is not available anymore (e.g. unplugged), keep using the image from the cache. if not available from the beginning, this will fail anyways
				{
					width = (int) (screen.Bounds.Width*region.Rectangle.Width);
					height = (int) (screen.Bounds.Height*region.Rectangle.Height);
					bitmap = _bitmapProvider.ProvideForScreenRegion(region, width, height, PixelFormat.Format24bppRgb);
				}
				else
				{
					width = 1;
					height = 1;
					bitmap = _blackBitmap;
				}

				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					if (screen == null)
					{
						graphics.FillRectangle(Brushes.Black, 0, 0, width, height);
					}
					else
					{
						var positionX = (int) (screen.Bounds.X + (screen.Bounds.Width*region.Rectangle.X));
						var positionY = (int) (screen.Bounds.Y + (screen.Bounds.Height*region.Rectangle.Y));
					
						graphics.CopyFromScreen(positionX, positionY, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
					}
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