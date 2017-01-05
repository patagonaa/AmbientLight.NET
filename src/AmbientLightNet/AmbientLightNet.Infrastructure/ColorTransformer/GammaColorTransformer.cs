using System;
using System.Collections.Generic;
using System.Globalization;
using AmbiLightNet.PluginBase;

namespace AmbientLightNet.Infrastructure.ColorTransformer
{
	public class GammaColorTransformer : IColorTransformer
	{
		private readonly float _gammaR;
		private readonly float _gammaG;
		private readonly float _gammaB;

		public GammaColorTransformer(IDictionary<string, object> config)
			: this(
				Convert.ToSingle(config["gammaR"], CultureInfo.InvariantCulture),
				Convert.ToSingle(config["gammaG"], CultureInfo.InvariantCulture),
				Convert.ToSingle(config["gammaB"], CultureInfo.InvariantCulture))
		{
		}

		public GammaColorTransformer(float gammaR, float gammaG, float gammaB)
		{
			_gammaR = gammaR;
			_gammaG = gammaG;
			_gammaB = gammaB;
		}

		public bool NeedsPreviousInputColors { get { return false; } }

		public ColorF Transform(ColorTransformerContext context, ColorF color)
		{
			var r = (float) Math.Pow(color.R, _gammaR);
			var g = (float) Math.Pow(color.G, _gammaG);
			var b = (float) Math.Pow(color.B, _gammaB);

			return ColorF.FromRgb(r, g, b);
		}
	}
}