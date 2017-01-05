using System;
using System.Drawing;
using AmbiLightNet.PluginBase;

namespace AmbientLightNet.Infrastructure.ColorAveraging
{
	public interface IColorAveragingService : IDisposable
	{
		ColorF GetAverageColor(Bitmap bitmap);
	}
}