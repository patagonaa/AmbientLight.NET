namespace AmbientLightNet.Infrastructure.Logging
{
	public static class StaticLogger
	{
		public static ILogger Logger { get; set; }

		public static void Log(LogLevel logLevel, string message)
		{
			if (Logger != null)
				Logger.Log(logLevel, message);
		}
		public static void Log(LogLevel logLevel, string message, object[] args)
		{
			if (Logger != null)
				Logger.Log(logLevel, message, args);
		}
	}
}
