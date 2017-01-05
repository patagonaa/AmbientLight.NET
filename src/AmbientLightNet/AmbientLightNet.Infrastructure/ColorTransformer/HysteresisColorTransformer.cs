using System;
using System.Collections.Generic;
using System.Globalization;
using AmbiLightNet.PluginBase;

namespace AmbientLightNet.Infrastructure.ColorTransformer
{
	public class HysteresisColorTransformer : IColorTransformer
	{
		private readonly float _hysteresis;

		public HysteresisColorTransformer(IDictionary<string, object> config)
			: this(Convert.ToSingle(config["hysteresis"], CultureInfo.InvariantCulture))
		{
		}

		public HysteresisColorTransformer(float hysteresis)
		{
			_hysteresis = hysteresis;
		}

		public bool NeedsPreviousInputColors { get { return false; } }

		public ColorF Transform(ColorTransformerContext context, ColorF color)
		{
			ColorF lastColor = context.PreviousOutputColor;

			float diffR = color.R - lastColor.R;
			float r = lastColor.R;

			if (diffR > _hysteresis)
				r += diffR - _hysteresis;

			if (diffR < -_hysteresis)
				r += diffR + _hysteresis;


			float diffG = color.G - lastColor.G;
			float g = lastColor.G;

			if (diffG > _hysteresis)
				g += diffG - _hysteresis;

			if (diffG < -_hysteresis)
				g += diffG + _hysteresis;


			float diffB = color.B - lastColor.B;
			float b = lastColor.B;

			if (diffB > _hysteresis)
				b += diffB - _hysteresis;

			if (diffB < -_hysteresis)
				b += diffB + _hysteresis;

			return ColorF.FromRgb(r, g, b);
		}
	}
}