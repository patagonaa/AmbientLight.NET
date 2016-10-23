using System.Drawing;

namespace AmbientLightNet.Infrastructure.ColorTransformer
{
	public interface IColorTransformer
	{
		Color Transform(Color color);
	}
}
