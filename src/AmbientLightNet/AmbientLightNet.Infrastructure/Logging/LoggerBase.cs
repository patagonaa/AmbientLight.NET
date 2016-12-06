namespace AmbientLightNet.Infrastructure.Logging
{
	public abstract class LoggerBase : ILogger
	{
		public abstract void Log(LogLevel logLevel, string message);

		public virtual void Log(LogLevel logLevel, string message, params object[] args)
		{
			Log(logLevel, string.Format(message, args));
		}
	}
}
