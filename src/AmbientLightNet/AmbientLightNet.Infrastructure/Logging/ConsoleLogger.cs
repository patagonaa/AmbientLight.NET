using System;

namespace AmbientLightNet.Infrastructure.Logging
{
	public class ConsoleLogger : LoggerBase
	{
		public override void Log(LogLevel logLevel, string message)
		{
			Console.WriteLine("[{0}] {1}", logLevel.ToString().ToUpperInvariant(), message);
		}
	}
}
