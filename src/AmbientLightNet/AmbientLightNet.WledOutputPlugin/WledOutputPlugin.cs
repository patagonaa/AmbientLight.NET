using System;
using AmbiLightNet.PluginBase;

namespace AmbientLightNet.SerialOutputPlugin
{
	public class WledOutputPlugin : IOutputPlugin
	{
		public const string PluginName = "Wled";

		public string DisplayName
		{
			get { return "WLED Output Plugin"; }
		}

		public string Name
		{
			get { return PluginName; }
		}

		public IOutputInfo GetNewOutputInfo()
		{
			return new WledOutputInfo();
		}

		public OutputService GetNewOutputService()
		{
			return new WledOutputService();
		}

		public OutputConfigDialog GetOutputConfigDialog()
		{
			throw new NotImplementedException();
		}
	}
}
