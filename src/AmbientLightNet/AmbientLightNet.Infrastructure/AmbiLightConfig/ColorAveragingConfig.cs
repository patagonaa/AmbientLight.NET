using System;
using System.Collections.Generic;

namespace AmbientLightNet.Infrastructure.AmbiLightConfig
{
	public class ColorAveragingConfig
	{
		public Type Type { get; set; }
		public IDictionary<string, object> Config { get; set; }
	}
}