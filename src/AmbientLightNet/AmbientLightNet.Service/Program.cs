using log4net.Config;
using Topshelf;

namespace AmbientLightNet.Service
{
	class Program
	{
		static void Main()
		{
			HostFactory.Run(x =>
			{
				XmlConfigurator.Configure();

				x.UseLog4Net();

				x.Service<AmbiLightService>(s =>
				{
					s.ConstructUsing(name => new AmbiLightService(ServiceConfiguration.ConfigFile));
					s.WhenStarted(service => service.Start());
					s.WhenStopped(service => service.Stop());
				});
				x.RunAsLocalSystem();

				x.EnableServiceRecovery(r =>
				{
					r.RestartService(1);
					r.RestartService(5);
					r.RestartService(10);
					r.OnCrashOnly();
					r.SetResetPeriod(0);
				});

				x.SetServiceName("AmbientLightNet.Service");
				x.SetDisplayName("AmbientLightNet Background Capture Service");
			});
		}
	}
}
