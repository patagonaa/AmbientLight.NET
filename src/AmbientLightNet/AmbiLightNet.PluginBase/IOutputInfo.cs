using System.Collections.Generic;

namespace AmbiLightNet.PluginBase
{
	public interface IOutputInfo
	{
		Dictionary<string, string> Serialize();
		void Deserialize(Dictionary<string, string> dictionary);
		string PluginName { get; }
	}
}