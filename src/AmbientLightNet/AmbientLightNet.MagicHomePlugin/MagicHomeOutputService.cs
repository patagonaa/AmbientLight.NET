using System;
using System.Linq;
using AmbiLightNet.PluginBase;
using MagicHomeController;

namespace AmbientLightNet.MagicHomePlugin
{
	public class MagicHomeOutputService : OutputService<MagicHomeLedOutput>
	{
		private Device _device;

		public override void Initialize(MagicHomeLedOutput outputType)
		{
			DeviceFindResult foundDevice = DeviceFinder.FindDevices().FirstOrDefault(x => x.MacAddress.Equals(outputType.MacAddress));
			if(foundDevice == null)
				throw new Exception(string.Format("device with mac {0} could not be found", outputType.MacAddress));

			_device = new Device(foundDevice.IpAddress, outputType.DeviceType);
			_device.TurnOn();
			_device.SetColor(0, 0, 0);
		}

		public override void Output(double r, double g, double b)
		{
			_device.SetColor((byte) (r*255), (byte) (g*255), (byte) (b*255));
		}

		public override void Dispose()
		{
			_device.Dispose();
			base.Dispose();
		}
	}
}
