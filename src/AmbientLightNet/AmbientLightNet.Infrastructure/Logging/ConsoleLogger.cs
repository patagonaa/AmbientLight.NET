using System;
using System.Globalization;

namespace AmbientLightNet.Infrastructure.Logging
{
	public class ConsoleLogger : LoggerBase
	{
		private readonly LogLevel _logLevel;

		public ConsoleLogger(LogLevel logLevel)
		{
			_logLevel = logLevel;
		}

		public override void Log(LogLevel logLevel, string message)
		{
			if(logLevel < _logLevel)
				return;

			Console.WriteLine("{0} - [{1}] {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture), logLevel.ToString().ToUpperInvariant(), message);
		}
	}
}
