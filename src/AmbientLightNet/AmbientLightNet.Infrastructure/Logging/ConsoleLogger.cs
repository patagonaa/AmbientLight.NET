using System;
using System.Globalization;

namespace AmbientLightNet.Infrastructure.Logging
{
	public class ConsoleLogger : LoggerBase
	{
		public override void Log(LogLevel logLevel, string message)
		{
			Console.WriteLine("{0} - [{1}] {2}", DateTime.Now.ToString("s", CultureInfo.InvariantCulture), logLevel.ToString().ToUpperInvariant(), message);
		}
	}
}
