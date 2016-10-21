using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AmbientLightNet.Infrastructure
{
	public class GdiScreenCaptureService : IScreenCaptureService
	{
		private readonly Dictionary<IList<ScreenRegion>, IList<Bitmap>> _bitmapCache;

		public GdiScreenCaptureService()
		{
			_bitmapCache = new Dictionary<IList<ScreenRegion>, IList<Bitmap>>(new ReferenceComparer<IList<ScreenRegion>>());
		}
		
		public IList<Bitmap> CaptureScreenRegions(IList<ScreenRegion> regions, bool useCache = false)
		{
			Screen[] allScreens = Screen.AllScreens;

			IList<Bitmap> bitmaps = new List<Bitmap>();

			IList<Bitmap> cachedCollection;
			bool cacheEntryExists = _bitmapCache.TryGetValue(regions, out cachedCollection);

			if (useCache && cacheEntryExists)
			{
				bitmaps = cachedCollection;
			}
			else
			{
				for (var i = 0; i < regions.Count; i++)
				{
					ScreenRegion region = regions[i];

					Screen screen = allScreens.Single(x => x.DeviceName == region.ScreenName);

					var width = (int)(screen.Bounds.Width * region.Rectangle.Width);
					var height = (int)(screen.Bounds.Height * region.Rectangle.Height);

					var bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
					bitmaps.Add(bitmap);
				}

				if (useCache)
				{
					cachedCollection = new List<Bitmap>();
					
					for (var i = 0; i < regions.Count; i++)
					{
						cachedCollection.Add(bitmaps[i]);
					}

					_bitmapCache[regions] = cachedCollection;
				}
			}

			var screenBitmaps = new Dictionary<Screen, Bitmap>();

			foreach (Screen screen in allScreens)
			{
				if(regions.All(x => x.ScreenName != screen.DeviceName))
					continue;

				Rectangle screenBounds = screen.Bounds;
				screenBitmaps[screen] = new Bitmap(screenBounds.Width, screenBounds.Height, PixelFormat.Format24bppRgb);
				using (Graphics g = Graphics.FromImage(screenBitmaps[screen]))
				{
					g.CopyFromScreen(screenBounds.Location, Point.Empty, screenBounds.Size, CopyPixelOperation.SourceCopy);
				}
			}
			
			for (var i = 0; i < regions.Count; i++)
			{
				ScreenRegion region = regions[i];
				Bitmap bitmap = bitmaps[i];

				Screen screen = allScreens.SingleOrDefault(x => x.DeviceName == region.ScreenName);

				if (screen == null)
				{
					using (Graphics g = Graphics.FromImage(bitmap))
					{
						g.FillRectangle(Brushes.Black, 0, 0, bitmap.Width, bitmap.Height);
					}
				}
				else
				{
					Rectangle screenBounds = screen.Bounds;
					RectangleF regionRect = region.Rectangle;

					var positionX = (int)(screenBounds.X + (screenBounds.Width * regionRect.X));
					var positionY = (int)(screenBounds.Y + (screenBounds.Height * regionRect.Y));

					ClipPart(bitmap, screenBitmaps[screen], positionX, positionY);
				}

				bitmaps.Add(bitmap);
			}

			foreach (KeyValuePair<Screen, Bitmap> screenBitmap in screenBitmaps)
			{
				screenBitmap.Value.Dispose();
			}

			return bitmaps;
		}

		[DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
		private static extern IntPtr memcpy(IntPtr dest, IntPtr src, uint count);

		private void ClipPart(Bitmap dest, Bitmap src, int positionX, int positionY)
		{
			BitmapData destData = dest.LockBits(new Rectangle(0, 0, dest.Width, dest.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
			BitmapData srcData = src.LockBits(new Rectangle(0, 0, src.Width, src.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

			const uint bytesPerPixel = 3;
			var rowBytes = (uint) (destData.Width * bytesPerPixel);
			int destHeight = dest.Height;
			for (var y = 0; y < destHeight; y++)
			{
				memcpy(destData.Scan0 + (destData.Stride * y), srcData.Scan0 + (srcData.Stride * (y + positionY) + positionX), rowBytes);
			}

			src.UnlockBits(srcData);
			dest.UnlockBits(destData);
		}

		public void ClearBitmapCache()
		{
			foreach (KeyValuePair<IList<ScreenRegion>, IList<Bitmap>> keyValuePair in _bitmapCache)
			{
				DisposeCacheEntry(keyValuePair.Value);
			}
			_bitmapCache.Clear();
		}

		private class ReferenceComparer<T> : IEqualityComparer<T>
		{
			public bool Equals(T x, T y)
			{
				return object.ReferenceEquals(x, y);
			}

			public int GetHashCode(T obj)
			{
				return obj == null ? 0 : obj.GetHashCode();
			}
		}

		public void Dispose()
		{
			foreach (KeyValuePair<IList<ScreenRegion>, IList<Bitmap>> keyValuePair in _bitmapCache)
			{
				DisposeCacheEntry(keyValuePair.Value);
			}
			_bitmapCache.Clear();
		}

		private void DisposeCacheEntry(IEnumerable<Bitmap> entry)
		{
			foreach (Bitmap bitmap in entry)
			{
				bitmap.Dispose();
			}
		}
	}
}