using System;
using System.IO;
using System.Threading;
using AmbientLightNet.Infrastructure.Logging;

namespace AmbientLightNet.Service
{
	class Program
	{
		static void Main(string[] args)
		{
			ILogger logger = new ConsoleLogger();

			if (args.Length < 1 || !File.Exists(args[0]))
			{
				logger.Log(LogLevel.Fatal, "Config Parameter with existing config expected!");
				return;
			}

			var watcher = new FileSystemWatcher(Path.GetDirectoryName(args[0]) == "" ? "." : Path.GetDirectoryName(args[0]), Path.GetFileName(args[0]));
			
			var ambiLight = new AmbiLightService(args[0], logger);

			watcher.Changed += (sender, eventArgs) =>
			{
				watcher.EnableRaisingEvents = false;
				while (FileIsLocked(eventArgs.FullPath))
				{
					Thread.Sleep(100);
				}
				ambiLight.ReloadConfig();
				watcher.EnableRaisingEvents = true;
			};
			watcher.EnableRaisingEvents = true;
			ambiLight.Start();
		}

		private static bool FileIsLocked(string filename)
		{
			try
			{
				using (File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
				{
					return false;
				}
			}
			catch (IOException)
			{
				return true;
			}
		}
	}
}
