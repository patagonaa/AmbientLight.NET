namespace AmbientLightNet.Infrastructure.Logging
{
	public interface ILogger
	{
		void Log(LogLevel logLevel, string message);
		void Log(LogLevel logLevel, string message, params object[] args);
	}
}
