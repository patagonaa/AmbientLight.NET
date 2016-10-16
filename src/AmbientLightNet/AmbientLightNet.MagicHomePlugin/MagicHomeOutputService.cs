using System;
using System.Drawing;
using System.Linq;
using AmbiLightNet.PluginBase;
using MagicHomeController;

namespace AmbientLightNet.MagicHomePlugin
{
	public class MagicHomeOutputService : OutputService<MagicHomeLedOutput>
	{
		private Device _device;
		private Color _lastColor;

		public override void Initialize(MagicHomeLedOutput outputType)
		{
			DeviceFindResult foundDevice = DeviceFinder.FindDevices().FirstOrDefault(x => x.MacAddress.Equals(outputType.MacAddress));
			if(foundDevice == null)
				throw new Exception(string.Format("device with mac {0} could not be found", outputType.MacAddress));

			_device = new Device(foundDevice.IpAddress, outputType.DeviceType);
			_device.TurnOn();
			Output(Color.Black);
		}

		public override void Output(Color color)
		{
			if(color == _lastColor)
				return;
			_device.SetColor(color);
			_lastColor = color;
		}

		public override void Dispose()
		{
			_device.Dispose();
			base.Dispose();
		}
	}
}
