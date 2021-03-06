using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AmbientLightNet.ScreenCapture.Infrastructure
{
	public class CachedScreenRegionBitmapProvider : IScreenRegionBitmapProvider
	{
		private readonly IDictionary<ScreenRegion, Bitmap> _cache;

		public CachedScreenRegionBitmapProvider()
		{
			_cache = new Dictionary<ScreenRegion, Bitmap>(new ReferenceComparer<ScreenRegion>());
		}

		public Bitmap ProvideForScreenRegion(ScreenRegion region, int width, int height, PixelFormat pixelFormat)
		{
			Bitmap bitmap;

			lock (_cache)
			{
				if (!_cache.TryGetValue(region, out bitmap))
					_cache[region] = bitmap = new Bitmap(width, height, pixelFormat);
			}

#if DEBUG
			if (BitmapIsDisposed(bitmap))
				throw new InvalidOperationException("Do not dispose cached Bitmaps!");
#endif

			return bitmap;
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
			lock (_cache)
			{
				foreach (Bitmap bitmap in _cache.Values)
				{
					bitmap.Dispose();
				}
			}
		}
	}
}