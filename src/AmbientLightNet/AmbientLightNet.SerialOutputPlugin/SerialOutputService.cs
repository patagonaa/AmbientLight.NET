using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO.Ports;
using System.Text;
using AmbiLightNet.PluginBase;

namespace AmbientLightNet.SerialOutputPlugin
{
	public class SerialOutputService : OutputService<SerialOutputInfo>
	{
		private SerialPort _port;
		private readonly IDictionary<int, Color> _pinColors;

		public SerialOutputService()
		{
			_pinColors = new Dictionary<int, Color>();
		}

		public override void Commit()
		{
			var toSend = new StringBuilder();

			foreach (var pinColor in _pinColors)
			{
				var color = pinColor.Value;

				toSend.AppendFormat(CultureInfo.InvariantCulture, "{0},{1},{2},{3};", pinColor.Key, color.R, color.G, color.B);
			}

			toSend.Append('!');

			_port.Write(toSend.ToString());

			_pinColors.Clear();
		}

		public override bool ColorsEqual(ColorF first, ColorF second)
		{
			return ((Color)first) == ((Color)second);
		}

		public override TimeSpan? GetResendInterval(int resendCount)
		{
			return null;
		}

		public override void Initialize(SerialOutputInfo outputInfo)
		{
			if (_port == null)
			{
				_port = new SerialPort(outputInfo.PortName, outputInfo.BaudRate)
				{
					Encoding = Encoding.ASCII
				};
				_port.Open();
			}
		}

		public override void SetColor(ColorF color, SerialOutputInfo outputInfo)
		{
			if(outputInfo.BaudRate != _port.BaudRate)
				throw new InvalidOperationException("BaudRate must be equal for all OutputInfos");

			_pinColors[outputInfo.LedNumber] = (Color) color;
		}

		public override bool IsReusableFor(SerialOutputInfo outputInfo)
		{
			return _port.PortName == outputInfo.PortName;
		}

		public override void Dispose()
		{
			base.Dispose();
			if (_port != null)
			{
				_port.Close();
				_port.Dispose();
				_port = null;
			}
		}
	}
}