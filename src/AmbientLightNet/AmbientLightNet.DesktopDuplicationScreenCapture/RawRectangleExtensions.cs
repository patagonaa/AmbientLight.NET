using System.Drawing;
using SharpDX.Mathematics.Interop;

namespace AmbientLightNet.DesktopDuplicationScreenCapture
{
	public static class RawRectangleExtensions
	{
		public static Rectangle ToRectangle(this RawRectangle rectangle)
		{
			return new Rectangle(rectangle.Left, rectangle.Top, rectangle.Right - rectangle.Left, rectangle.Bottom - rectangle.Top);
		}
	}
}
