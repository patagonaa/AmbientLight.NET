using System;
using System.Drawing;
using System.Linq;
using AmbiLightNet.PluginBase;
using MagicHomeController;
using System.Net;

namespace AmbientLightNet.MagicHomePlugin
{
	public class MagicHomeOutputService : SimpleOutputService<MagicHomeLedOutputInfo>
	{
		private Device _device;

		public override void Initialize(MagicHomeLedOutputInfo outputInfo)
		{
			IPAddress ip;
			int port = 5577;

			switch (outputInfo.AddressType)
			{
				case AddressType.MacAddress:
					DeviceFindResult foundDevice = DeviceFinder.FindDevices().FirstOrDefault(x => x.MacAddress.Equals(outputInfo.MacAddress));
					if (foundDevice == null)
						throw new Exception(string.Format("device with mac {0} could not be found", outputInfo.MacAddress));
					ip = foundDevice.IpAddress;
					break;

				case AddressType.IpAddress:
					ip = outputInfo.IPAddress;
					if (outputInfo.Port.HasValue)
						port = outputInfo.Port.Value;
					break;
				default:
					throw new IndexOutOfRangeException();
			}

			_device = new Device(new IPEndPoint(ip, port), outputInfo.DeviceType);
			_device.TurnOn();
		}

		protected override void Output(ColorF colorF)
		{
			var color = (Color) colorF;

			_device.SetColor(color.R, color.G, color.B, waitForResponse: false, persist: false);
		}

		public override bool ColorsEqual(ColorF first, ColorF second)
		{
			return ((Color) first) == ((Color) second);
		}

		public override TimeSpan? GetResendInterval(int resendCount)
		{
			if (resendCount == 0)
			{
				return TimeSpan.FromMilliseconds(100);
			}
			if (resendCount > 0 && resendCount <= 10)
			{
				return TimeSpan.FromSeconds(1);
			}
			if (resendCount > 10 && resendCount <= 20)
			{
				return TimeSpan.FromSeconds(10);
			}
			return TimeSpan.FromSeconds(60);
		}

		public override void Dispose()
		{
			_device.Dispose();
			base.Dispose();
		}
	}
}
