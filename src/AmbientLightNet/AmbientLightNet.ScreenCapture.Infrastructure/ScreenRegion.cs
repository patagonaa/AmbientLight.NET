namespace AmbientLightNet.ScreenCapture.Infrastructure
{
	public class ScreenRegion
	{
		public ScreenRegion(string screenName, RectangleFSerializable rectangle)
		{
			ScreenName = screenName;
			Rectangle = rectangle;
		}

		public string ScreenName { get; private set; }
		public RectangleFSerializable Rectangle { get; private set; }

		public override string ToString()
		{
			return string.Format("{0}: {1}", ScreenName, Rectangle);
		}
	}
}