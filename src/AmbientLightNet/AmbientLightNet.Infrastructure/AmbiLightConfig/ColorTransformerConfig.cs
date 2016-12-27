using System;
using System.Collections.Generic;

namespace AmbientLightNet.Infrastructure.AmbiLightConfig
{
	public class ColorTransformerConfig
	{
		public Type Type { get; set; }
		public IDictionary<string, object> Config { get; set; }
	}
}