using System.Collections.Generic;
using System.Globalization;
using AmbiLightNet.PluginBase;

namespace AmbientLightNet.SerialOutputPlugin
{
	public class SerialOutputInfo : IOutputInfo
	{
		public Dictionary<string, string> Serialize()
		{
			return new Dictionary<string, string>
			{
				{"PortName", PortName},
				{"LedNumber", LedNumber.ToString(CultureInfo.InvariantCulture)},
				{"BaudRate", BaudRate.ToString(CultureInfo.InvariantCulture)}
			};
		}

		public void Deserialize(Dictionary<string, string> dictionary)
		{
			PortName = dictionary["PortName"];
			LedNumber = int.Parse(dictionary["LedNumber"]);
			BaudRate = int.Parse(dictionary["BaudRate"]);
		}

		public string PluginName
		{
			get { return SerialOutputPlugin.PluginName; }
		}
		
		public string PortName { get; set; }
		public int LedNumber { get; set; }
		public int BaudRate { get; set; }
	}
}