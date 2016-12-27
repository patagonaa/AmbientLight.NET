using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AmbientLightNet.Infrastructure.AmbiLightConfig;
using AmbientLightNet.Infrastructure.ColorTransformer;

namespace AmbientLightNet.Service
{
	public class ColorTransformerProvider
	{
		public IList<IColorTransformer> Provide(IList<ColorTransformerConfig> config)
		{
			if (config == null)
			{
				return new List<IColorTransformer>
				{
					new BrightnessColorTransformer(1, 0.9, 0.4),
					new GammaColorTransformer(1, 1.2, 1.2)
				};
			}

			return config.Select(x =>
			{
				ConstructorInfo ctor = x.Type.GetConstructor(new[] { typeof(IDictionary<string, object>) });

				if (ctor == null)
					throw new InvalidOperationException(string.Format("Color Transformer {0} is missing constructor with config", x.Type.Name));

				return (IColorTransformer) ctor.Invoke(new object[] {x.Config});
			}).ToList();
		}
	}
}