using System;
using System.Collections.Generic;
using System.Globalization;
using AmbiLightNet.PluginBase;

namespace AmbientLightNet.Infrastructure.ColorTransformer
{
	public class BrightnessColorTransformer : IColorTransformer
	{
		private readonly float _factorR;
		private readonly float _factorG;
		private readonly float _factorB;

		public BrightnessColorTransformer(IDictionary<string, object> config)
			: this(
				Convert.ToSingle(config["factorR"], CultureInfo.InvariantCulture),
				Convert.ToSingle(config["factorG"], CultureInfo.InvariantCulture),
				Convert.ToSingle(config["factorB"], CultureInfo.InvariantCulture))
		{
		}

		public BrightnessColorTransformer(float factorR, float factorG, float factorB)
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

		public bool NeedsPreviousInputColors { get { return false; } }

		public ColorF Transform(ColorTransformerContext context, ColorF color)
		{
			return ColorF.FromRgb(color.R*_factorR, color.G*_factorG, color.B*_factorB);
		}
	}
}
