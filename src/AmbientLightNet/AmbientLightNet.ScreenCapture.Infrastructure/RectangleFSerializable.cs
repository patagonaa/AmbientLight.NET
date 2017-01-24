using System.Drawing;
using Newtonsoft.Json;

namespace AmbientLightNet.ScreenCapture.Infrastructure
{
	public struct RectangleFSerializable
	{
		public RectangleFSerializable(float x, float y, float width, float height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}

		public float X { get; set; }
		public float Y { get; set; }
		public float Width { get; set; }
		public float Height { get; set; }

		[JsonIgnore]
		public PointF Location
		{
			get { return new PointF(X, Y); }
		}

		[JsonIgnore]
		public SizeF Size
		{
			get { return new SizeF(Width, Height); }
		}
	}
}
