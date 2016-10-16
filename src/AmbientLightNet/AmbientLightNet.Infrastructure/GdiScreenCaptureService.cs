using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AmbientLightNet.Infrastructure
{
	public class GdiScreenCaptureService : IScreenCaptureService
	{
		private readonly Dictionary<IList<ScreenRegion>, IList<KeyValuePair<Bitmap, Graphics>>> _bitmapCache;

		public GdiScreenCaptureService()
		{
			_bitmapCache = new Dictionary<IList<ScreenRegion>, IList<KeyValuePair<Bitmap, Graphics>>>(new ReferenceComparer<IList<ScreenRegion>>());
		}
		
		public IList<Bitmap> CaptureScreenRegions(IList<ScreenRegion> regions, bool useCache = false)
		{
			Screen[] allScreens = Screen.AllScreens;

			IList<Bitmap> bitmaps = new List<Bitmap>();
			IList<Graphics> graphicses = new List<Graphics>();

			IList<KeyValuePair<Bitmap, Graphics>> cachedCollection;
			bool cacheEntryExists = _bitmapCache.TryGetValue(regions, out cachedCollection);

			if (useCache && cacheEntryExists)
			{
				bitmaps = cachedCollection.Select(x => x.Key).ToList();
				if(bitmaps.Any(BitmapIsDisposed))
					throw new InvalidOperationException("Do not dispose cached Bitmaps!");
				graphicses = cachedCollection.Select(x => x.Value).ToList();
			}
			else
			{
				for (var i = 0; i < regions.Count; i++)
				{
					ScreenRegion region = regions[i];

					Screen screen = allScreens.Single(x => x.DeviceName == region.ScreenName);

					var width = (int)(screen.Bounds.Width * region.Rectangle.Width);
					var height = (int)(screen.Bounds.Height * region.Rectangle.Height);

					var bitmap = new Bitmap(width, height);
					bitmaps.Add(bitmap);
					graphicses.Add(Graphics.FromImage(bitmap));
				}

				if (useCache)
				{
					cachedCollection = new List<KeyValuePair<Bitmap, Graphics>>();
					
					for (var i = 0; i < regions.Count; i++)
					{
						cachedCollection.Add(new KeyValuePair<Bitmap, Graphics>(bitmaps[i], graphicses[i]));
					}

					_bitmapCache[regions] = cachedCollection;
				}
			}
			
			for (var i = 0; i < regions.Count; i++)
			{
				ScreenRegion region = regions[i];
				Bitmap bitmap = bitmaps[i];
				Graphics graphics = graphicses[i];

				Screen screen = allScreens.Single(x => x.DeviceName == region.ScreenName);

				var positionX = (int)(screen.Bounds.X + (screen.Bounds.Width * region.Rectangle.X));
				var positionY = (int)(screen.Bounds.Y + (screen.Bounds.Height * region.Rectangle.Y));

				var width = (int)(screen.Bounds.Width * region.Rectangle.Width);
				var height = (int)(screen.Bounds.Height * region.Rectangle.Height);

				graphics.CopyFromScreen(positionX, positionY, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);

				bitmaps.Add(bitmap);
			}

			return bitmaps;
		}

		public void ClearBitmapCache()
		{
			foreach (KeyValuePair<IList<ScreenRegion>, IList<KeyValuePair<Bitmap, Graphics>>> keyValuePair in _bitmapCache)
			{
				DisposeCacheEntry(keyValuePair.Value);
			}
			_bitmapCache.Clear();
		}

		private static bool BitmapIsDisposed(Bitmap bitmap)
		{
			try
			{
				bitmap.GetPixel(0, 0);
				return false;
			}
			catch (ArgumentException)
			{
				return true;
			}
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
			foreach (KeyValuePair<IList<ScreenRegion>, IList<KeyValuePair<Bitmap, Graphics>>> keyValuePair in _bitmapCache)
			{
				DisposeCacheEntry(keyValuePair.Value);
			}
			_bitmapCache.Clear();
		}

		private void DisposeCacheEntry(IEnumerable<KeyValuePair<Bitmap, Graphics>> entry)
		{
			foreach (KeyValuePair<Bitmap, Graphics> keyValuePair in entry)
			{
				keyValuePair.Key.Dispose();
				keyValuePair.Value.Dispose();
			}
		}
	}
}