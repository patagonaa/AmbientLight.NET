using System.Configuration;

namespace AmbientLightNet.Infrastructure
{
	public static class AppConfiguration
	{
		public static string PluginDirectory
		{
			get { return ConfigurationManager.AppSettings["OutputPluginDirectory"]; }
		}
	}
}
