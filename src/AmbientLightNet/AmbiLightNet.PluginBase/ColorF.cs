using System;
using System.Drawing;

namespace AmbiLightNet.PluginBase
{
	public struct ColorF : IEquatable<ColorF>
	{
		private readonly float _r;
		private readonly float _g;
		private readonly float _b;

		private ColorF(float r, float g, float b)
		{
			if(r < 0f || r > 1f)
				throw new ArgumentOutOfRangeException("r", "values must be between 0 and 255");
			if(g < 0f || g > 1f)
				throw new ArgumentOutOfRangeException("g", "values must be between 0 and 255");
			if(b < 0f || b > 1f)
				throw new ArgumentOutOfRangeException("b", "values must be between 0 and 255");
			
			_r = r;
			_g = g;
			_b = b;
		}

		public float R
		{
			get { return _r; }
		}

		public float G
		{
			get { return _g; }
		}

		public float B
		{
			get { return _b; }
		}

		public static ColorF FromRgb(float r, float g, float b)
		{
			return new ColorF(r, g, b);
		}

		public float GetBrightness()
		{
			return (R + G + B) / 3;
		}
		
		public static explicit operator ColorF(Color color)
		{
			return FromRgb(color.R/255f, color.G/255f, color.B/255f);
		}

		public static explicit operator Color(ColorF colorF)
		{
			var r = (int) (colorF.R*255f);
			var g = (int) (colorF.G*255f);
			var b = (int) (colorF.B*255f);

			return Color.FromArgb(255, r, g, b);
		}

		public override bool Equals(object obj)
		{
			return obj is ColorF && Equals((ColorF) obj);
		}

		public bool Equals(ColorF other)
		{
			return R.Equals(other.R) && G.Equals(other.G) && B.Equals(other.B);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = _r.GetHashCode();
				hashCode = (hashCode*397) ^ _g.GetHashCode();
				hashCode = (hashCode*397) ^ _b.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(ColorF left, ColorF right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ColorF left, ColorF right)
		{
			return !left.Equals(right);
		}

		public bool Equals(ColorF colorF, float tolerance)
		{
			return Math.Abs(R - colorF.R) < tolerance &&
			       Math.Abs(G - colorF.G) < tolerance &&
			       Math.Abs(B - colorF.B) < tolerance;
		}

		public override string ToString()
		{
			return string.Format("#{0:X2}{1:X2}{2:X2}", (int)(R * 255), (int)(G * 255), (int)(B * 255));
		}
	}
}
