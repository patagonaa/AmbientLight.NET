using System;
using System.Drawing;

namespace AmbientLightNet.Infrastructure.ColorAveraging
{
	public interface IColorAveragingService : IDisposable
	{
		Color GetAverageColor(Bitmap bitmap);
	}
}