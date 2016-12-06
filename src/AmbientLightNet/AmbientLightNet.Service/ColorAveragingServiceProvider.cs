using System;
using System.Collections.Generic;
using System.Reflection;
using AmbientLightNet.Infrastructure.AmbiLightConfig;
using AmbientLightNet.Infrastructure.ColorAveraging;

namespace AmbientLightNet.Service
{
	public class ColorAveragingServiceProvider
	{
		public IColorAveragingService Provide(ColorAveragingConfig config)
		{
			if (config == null)
				return new GdiFastPixelAveraging(10, 10);
			
			ConstructorInfo ctor = config.Type.GetConstructor(new[] {typeof (IDictionary<string, object>)});

			if (ctor == null)
				throw new InvalidOperationException(string.Format("Color Averaging Service {0} is missing constructor with config", config.Type.Name));

			return (IColorAveragingService) ctor.Invoke(new object[] {config.Config});
		}
	}
}