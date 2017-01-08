using System.Configuration;

namespace AmbientLightNet.Infrastructure
{
	public static class CommonConfiguration
	{
		public static string PluginDirectory
		{
			get { return ConfigurationManager.AppSettings["OutputPluginDirectory"]; }
		}
	}
}
