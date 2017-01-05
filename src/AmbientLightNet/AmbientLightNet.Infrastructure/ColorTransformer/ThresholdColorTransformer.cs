using System;
using System.Collections.Generic;
using System.Globalization;
using AmbiLightNet.PluginBase;

namespace AmbientLightNet.Infrastructure.ColorTransformer
{
	public class ThresholdColorTransformer : IColorTransformer
	{
		private readonly double _threshold;

		public ThresholdColorTransformer(IDictionary<string, object> config)
			: this(Convert.ToSingle(config["threshold"], CultureInfo.InvariantCulture))
		{
		}

		public ThresholdColorTransformer(float threshold)
		{
			if(threshold < 0 || threshold > 1)
				throw new ArgumentOutOfRangeException("threshold", "threshold must be between 0 and 1");

			_threshold = threshold;
		}

		public bool NeedsPreviousInputColors { get { return false; } }

		public ColorF Transform(ColorTransformerContext context, ColorF color)
		{
			float brightness = color.GetBrightness();

			if (brightness > (1f - _threshold))
				return ColorF.FromRgb(1f, 1f, 1f);

			if (brightness < _threshold)
				return ColorF.FromRgb(0f, 0f, 0f);

			return color;
		}
	}
}