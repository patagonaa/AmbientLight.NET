using System.Configuration;

namespace AmbientLightNet.Service
{
	public static class ServiceConfiguration
	{
		public static string ConfigFile
		{
			get { return ConfigurationManager.AppSettings["ConfigFile"]; }
		}
	}
}
