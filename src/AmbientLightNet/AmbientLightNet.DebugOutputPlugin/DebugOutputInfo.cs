using System;
using System.Collections.Generic;
using AmbiLightNet.PluginBase;

namespace AmbientLightNet.DebugOutputPlugin
{
	public class DebugOutputInfo : IOutputInfo
	{
		public Dictionary<string, string> Serialize()
		{
			return new Dictionary<string, string>()
			{
				{"Id", Id.ToString()}
			};
		}

		public void Deserialize(Dictionary<string, string> dictionary)
		{
			Id = Guid.Parse(dictionary["Id"]);
		}

		public Guid Id { get; set; }
		public string PluginName { get { return DebugOutputPlugin.PluginName; } }
	}
}