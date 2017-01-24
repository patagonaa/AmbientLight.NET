using System;
using AmbiLightNet.PluginBase;

namespace AmbientLightNet.MagicHomePlugin
{
	public class MagicHomeOutputPlugin : IOutputPlugin
	{
		public const string PluginName = "MagicHome";

		public string DisplayName
		{
			get { return "MagicHomeController.NET"; }
		}

		public string Name
		{
			get { return PluginName; }
		}

		public IOutputInfo GetNewOutputInfo()
		{
			return new MagicHomeLedOutputInfo();
		}

		public OutputService GetNewOutputService()
		{
			return new MagicHomeOutputService();
		}

		public OutputConfigDialog GetOutputConfigDialog()
		{
			return new MagicHomeOutputConfigDialog();
		}
	}
}