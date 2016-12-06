using System;
using System.Collections.Generic;
using System.Drawing;

namespace AmbientLightNet.Infrastructure.ColorTransformer
{
	public class GammaColorTransformer : IColorTransformer
	{
		private readonly double _gammaR;
		private readonly double _gammaG;
		private readonly double _gammaB;

		public GammaColorTransformer(IDictionary<string, object> config)
			: this(
				(double) config["gammaR"],
				(double) config["gammaG"],
				(double) config["gammaB"])
		{
		}

		public GammaColorTransformer(double gammaR, double gammaG, double gammaB)
		{
			_gammaR = gammaR;
			_gammaG = gammaG;
			_gammaB = gammaB;
		}

		public Color Transform(Color color)
		{
			double r = Math.Pow(color.R/255d, _gammaR)*255;
			double g = Math.Pow(color.G/255d, _gammaG)*255;
			double b = Math.Pow(color.B/255d, _gammaB)*255;

			return Color.FromArgb((int) r, (int) g, (int) b);
		}
	}
}