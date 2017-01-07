using System;
using System.Collections.Generic;
using System.Reflection;
using AmbiLightNet.PluginBase;

namespace AmbientLightNet.Infrastructure.ColorTransformer
{
	public class ColorTransformerService : IDisposable
	{
		private readonly IDictionary<ColorTransformerContext, IColorTransformer> _transformers;

		public ColorTransformerService()
		{
			_transformers = new Dictionary<ColorTransformerContext, IColorTransformer>();
		}

		public ColorF Transform(IList<ColorTransformerContext> colorTransformerContexts, ColorF color)
		{
			var currentColor = color;

			foreach (ColorTransformerContext colorTransformerContext in colorTransformerContexts)
			{
				IColorTransformer transformer = GetTransformer(colorTransformerContext);

				ColorF oldColor = currentColor;

				currentColor = transformer.Transform(colorTransformerContext, currentColor);

				colorTransformerContext.PreviousOutputColor = currentColor;

				if (transformer.NeedsPreviousInputColors)
				{
					colorTransformerContext.PreviousInputColors.Add(oldColor);
					
					if (colorTransformerContext.PreviousInputColors.Count > 10000)
					{
#if DEBUG
						throw new InvalidOperationException("What do you need that many samples for? use colorTransformerContext.PreviousInputColors.RemoveAt(0); to remove previous color samples");
#endif
					}
				}
			}

			return currentColor;
		}

		private IColorTransformer GetTransformer(ColorTransformerContext context)
		{
			IColorTransformer toReturn;

			lock (_transformers)
			{
				if (!_transformers.TryGetValue(context, out toReturn))
				{
					ConstructorInfo ctor = context.Type.GetConstructor(new[] { typeof(IDictionary<string, object>) });

					if (ctor == null)
						throw new InvalidOperationException(string.Format("Color Transformer {0} is missing constructor with config", context.Type.Name));

					_transformers[context] = toReturn = (IColorTransformer)ctor.Invoke(new object[] { context.TransformerConfig });
				}
			}
			return toReturn;
		}

		public void Dispose()
		{
			lock (_transformers)
			{
				_transformers.Clear();
			}
		}
	}
}