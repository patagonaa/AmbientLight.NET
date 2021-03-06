﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AmbiLightNet.PluginBase;

namespace AmbientLightNet.Infrastructure
{
	public static class OutputPlugins
	{
		private static readonly List<IOutputPlugin> AvailablePlugins;

		static OutputPlugins()
		{
			AvailablePlugins = GetAllPlugins();
		}

		private static List<IOutputPlugin> GetAllPlugins()
		{
			Type interfaceType = typeof (IOutputPlugin);

			DirectoryInfo directory = new DirectoryInfo(CommonConfiguration.PluginDirectory);
			if (!directory.Exists)
				directory.Create();

			IList<Assembly> loadedAssemblies = Directory.EnumerateFiles(directory.FullName, "*.dll").Select(Assembly.LoadFile).ToList();
			IList<Type> pluginTypes = loadedAssemblies
				.SelectMany(x => x.GetTypes())
				.Where(x => interfaceType.IsAssignableFrom(x) && x.IsClass).ToList();

			return pluginTypes.Select(x => (IOutputPlugin) Activator.CreateInstance(x)).ToList();
		}

		public static List<IOutputPlugin> GetAvailablePlugins()
		{
			return AvailablePlugins;
		}

		public static IOutputPlugin GetOutputPlugin(string pluginName)
		{
			IOutputPlugin toReturn = AvailablePlugins.SingleOrDefault(x => x.Name == pluginName);

			if (toReturn == null)
				throw new Exception(string.Format("Output plugin {0} not found", pluginName));

			return toReturn;
		}
	}
}
