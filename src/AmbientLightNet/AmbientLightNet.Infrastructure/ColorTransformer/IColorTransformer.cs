using AmbiLightNet.PluginBase;

namespace AmbientLightNet.Infrastructure.ColorTransformer
{
	public interface IColorTransformer
	{
		bool NeedsPreviousInputColors { get; }
		ColorF Transform(ColorTransformerContext context, ColorF color);
	}
}
