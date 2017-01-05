using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AmbientLightNet.Infrastructure.AmbiLightConfig;
using AmbiLightNet.PluginBase;

namespace AmbientLightNet.Infrastructure.ColorTransformer
{
	public class ColorTransformerContext
	{
		private readonly Type _type;
		private readonly IReadOnlyDictionary<string, object> _transformerConfig;
		private readonly IList<ColorF> _previousInputColors;

		public ColorTransformerContext(ColorTransformerConfig colorTransformerConfig)
		{
			_type = colorTransformerConfig.Type;
			_transformerConfig = new ReadOnlyDictionary<string, object>(colorTransformerConfig.Config);
			_previousInputColors = new List<ColorF>();
		}

		public Type Type
		{
			get { return _type; }
		}

		public IReadOnlyDictionary<string, object> TransformerConfig
		{
			get { return _transformerConfig; }
		}

		public IList<ColorF> PreviousInputColors
		{
			get { return _previousInputColors; }
		}

		public ColorF PreviousOutputColor { get; set; }
	}
}