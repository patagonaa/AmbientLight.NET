using System;

namespace AmbientLightNet.DesktopDuplicationScreenCapture
{
	public class ScreenNotFoundException : Exception
	{
		public ScreenNotFoundException(string screenName) 
			: base(string.Format("Screen \"{0}\" not found", screenName))
		{
			ScreenName = screenName;
		}

		public string ScreenName { get; set; }
	}
}
