using AmbiLightNet.PluginBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AmbientLightNet.Infrastructure.AmbiLightConfig
{
	[Serializable]
	public class Output : ISerializable
	{
		public IOutputInfo OutputInfo { get; set; }
		public IList<ColorTransformerConfig> ColorTransformerConfigs { get; set; }
		

		public Output()
		{
		}

		protected Output(SerializationInfo info, StreamingContext context)
		{
			try
			{
				ColorTransformerConfigs = (IList<ColorTransformerConfig>)info.GetValue("ColorTransformerConfigs", typeof(IList<ColorTransformerConfig>));
			}
			catch (SerializationException)
			{
			}

			string pluginName = info.GetString("OutputPluginName");

			if (!string.IsNullOrEmpty(pluginName))
			{
				IOutputPlugin outputPlugin = OutputPlugins.GetOutputPlugin(pluginName);
				if (outputPlugin == null)
					throw new SerializationException(string.Format("Error while deserializing: Output plugin {0} not found", pluginName));

				IOutputInfo outputInfo = outputPlugin.GetNewOutputInfo();
				outputInfo.Deserialize((Dictionary<string, string>)info.GetValue("OutputInfoDictionary", typeof(Dictionary<string, string>)));

				OutputInfo = outputInfo;
			}
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (OutputInfo != null)
			{
				info.AddValue("OutputPluginName", OutputInfo.PluginName);
				info.AddValue("OutputInfoDictionary", OutputInfo.Serialize());
			}

			info.AddValue("ColorTransformerConfigs", ColorTransformerConfigs, typeof(IList<ColorTransformerConfig>));
			
		}
	}
}
