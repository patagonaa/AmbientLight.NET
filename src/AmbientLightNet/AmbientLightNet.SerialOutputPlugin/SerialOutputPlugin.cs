using System;
using AmbiLightNet.PluginBase;

namespace AmbientLightNet.SerialOutputPlugin
{
	public class SerialOutputPlugin : IOutputPlugin
	{
		public const string PluginName = "Serial";

		public string DisplayName
		{
			get { return "Serial Output Plugin"; }
		}

		public string Name
		{
			get { return PluginName; }
		}

		public IOutputInfo GetNewOutputInfo()
		{
			return new SerialOutputInfo();
		}

		public OutputService GetNewOutputService()
		{
			return new SerialOutputService();
		}

		public OutputConfigDialog GetOutputConfigDialog()
		{
			throw new NotImplementedException();
		}
	}
}
