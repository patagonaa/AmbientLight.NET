using System;
using System.Drawing;

namespace AmbientLightNet.Infrastructure.ColorTransformer
{
	public class BrightnessColorTransformer : IColorTransformer
	{
		private readonly double _factorR;
		private readonly double _factorG;
		private readonly double _factorB;

		public BrightnessColorTransformer(double factorR, double factorG, double factorB)
		{
			if(factorR < 0 || factorR > 1)
				throw new ArgumentException("factor must be between 0 and 1", "factorR");
			if(factorG < 0 || factorG > 1)
				throw new ArgumentException("factor must be between 0 and 1", "factorG");
			if(factorB < 0 || factorB > 1)
				throw new ArgumentException("factor must be between 0 and 1", "factorB");

			_factorR = factorR;
			_factorG = factorG;
			_factorB = factorB;
		}

		public Color Transform(Color color)
		{
			return Color.FromArgb((int) (color.R*_factorR), (int) (color.G*_factorG), (int) (color.B*_factorB));
		}
	}
}
