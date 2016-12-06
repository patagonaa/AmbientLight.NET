using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using AmbiLightNet.PluginBase;

namespace AmbientLightNet.Infrastructure.AmbiLightConfig
{
	[Serializable]
	public class ScreenRegionOutput : ISerializable
	{
		public ScreenRegion ScreenRegion { get; set; }
		public IOutputInfo OutputInfo { get; set; }
		public IList<ColorTransformerConfig> ColorTransformerConfigs { get; set; }
		public ColorAveragingConfig ColorAveragingConfig { get; set; }

		public ScreenRegionOutput()
		{
		}

		protected ScreenRegionOutput(SerializationInfo info, StreamingContext context)
		{
			ScreenRegion = (ScreenRegion)info.GetValue("ScreenRegion", typeof(ScreenRegion));

			try
			{
				ColorTransformerConfigs = (IList<ColorTransformerConfig>)info.GetValue("ColorTransformerConfigs", typeof(IList<ColorTransformerConfig>));
				ColorAveragingConfig = (ColorAveragingConfig)info.GetValue("ColorAveragingConfig", typeof(ColorAveragingConfig));
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
			info.AddValue("ScreenRegion", ScreenRegion);
			if (OutputInfo != null)
			{
				info.AddValue("OutputPluginName", OutputInfo.PluginName);
				info.AddValue("OutputInfoDictionary", OutputInfo.Serialize());
			}

			info.AddValue("ColorTransformerConfigs", ColorTransformerConfigs, typeof(IList<ColorTransformerConfig>));
			info.AddValue("ColorAveragingConfig", ColorAveragingConfig);
		}

		public override string ToString()
		{
			return string.Format("{0} Output: {1}", ScreenRegion, OutputInfo == null ? "None" : OutputInfo.ToString());
		}
	}
}