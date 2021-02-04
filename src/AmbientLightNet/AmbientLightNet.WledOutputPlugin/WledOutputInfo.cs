using System.Collections.Generic;
using System.Net;
using AmbiLightNet.PluginBase;

namespace AmbientLightNet.SerialOutputPlugin
{
    public class WledOutputInfo : IOutputInfo
	{
		public Dictionary<string, string> Serialize()
		{
			return new Dictionary<string, string>
			{
				{"IPAddress", IpAddress.ToString()},
				{"LedNumber", LedNumber.ToString()}
			};
		}

		public void Deserialize(Dictionary<string, string> dictionary)
		{
			IpAddress = IPAddress.Parse(dictionary["IPAddress"]);
			LedNumber = int.TryParse(dictionary["LedNumber"], out var ledNum) ? ledNum : 0;
		}

		public string PluginName
		{
			get { return WledOutputPlugin.PluginName; }
		}

		public IPAddress IpAddress { get; set; }
        public int LedNumber { get; set; }
    }
}