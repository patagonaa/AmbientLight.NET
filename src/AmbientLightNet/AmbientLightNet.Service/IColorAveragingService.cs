using System;
using System.Drawing;

namespace AmbientLightNet.Service
{
	internal interface IColorAveragingService : IDisposable
	{
		Color GetAverageColor(Bitmap bitmap);
	}
}