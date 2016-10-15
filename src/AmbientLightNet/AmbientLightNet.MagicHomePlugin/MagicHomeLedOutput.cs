using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using AmbiLightNet.PluginBase;
using MagicHomeController;

namespace AmbientLightNet.MagicHomePlugin
{
	public class MagicHomeLedOutput : IOutputInfo
	{
		public PhysicalAddress MacAddress { get; set; }
		public DeviceType DeviceType { get; set; }

		public Dictionary<string, string> Serialize()
		{
			return new Dictionary<string, string>
			{
				{"MacAddress", MacAddress.ToString()},
				{"DeviceType", ((int)DeviceType).ToString()}
			};
		}

		public void Deserialize(Dictionary<string, string> dictionary)
		{
			MacAddress = PhysicalAddress.Parse(dictionary["MacAddress"]);
			DeviceType = (DeviceType) Enum.Parse(typeof (DeviceType), dictionary["DeviceType"]);
		}

		public string PluginName { get { return MagicHomeOutputPlugin.PluginName; } }

		public override string ToString()
		{
			return string.Format("MagicHome: {0}", MacAddress);
		}
	}
}