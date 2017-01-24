using System.Collections.Generic;
using System.Globalization;
using System.Net.NetworkInformation;
using AmbiLightNet.PluginBase;
using MagicHomeController;
using System;
using System.Net;

namespace AmbientLightNet.MagicHomePlugin
{
	public class MagicHomeLedOutputInfo : IOutputInfo
	{
		public AddressType AddressType { get; set; }
		public int? Port { get; set; }

		public PhysicalAddress MacAddress { get; set; }
		public IPAddress IPAddress { get; set; }
		public DeviceType DeviceType { get; set; }

		public Dictionary<string, string> Serialize()
		{
			var toReturn = new Dictionary<string, string>
			{
				{"DeviceType", DeviceType.ToString()},
				{"AddressType", AddressType.ToString()}
			};

			switch (AddressType)
			{
				case AddressType.MacAddress:
					toReturn["MacAddress"] = MacAddress.ToString();
					break;
				case AddressType.IpAddress:
					toReturn["IPAddress"] = IPAddress.ToString();
					if (Port.HasValue)
						toReturn["Port"] = Port.Value.ToString(CultureInfo.InvariantCulture);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return toReturn;
		}

		public void Deserialize(Dictionary<string, string> dictionary)
		{
			DeviceType = (DeviceType) Enum.Parse(typeof (DeviceType), dictionary["DeviceType"]);
			AddressType = AddressType.MacAddress;

			if (dictionary.ContainsKey("AddressType"))
			{
				AddressType = (AddressType) Enum.Parse(typeof (AddressType), dictionary["AddressType"]);
			}

			switch (AddressType)
			{
				case AddressType.IpAddress:
					IPAddress = IPAddress.Parse(dictionary["IPAddress"]);
					Port = dictionary.ContainsKey("Port") ? int.Parse(dictionary["Port"]) : (int?) null;
					break;
				case AddressType.MacAddress:
					MacAddress = PhysicalAddress.Parse(dictionary["MacAddress"]);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public string PluginName
		{
			get { return MagicHomeOutputPlugin.PluginName; }
		}

		public override string ToString()
		{
			return string.Format("MagicHome: {0}", MacAddress);
		}
	}
}