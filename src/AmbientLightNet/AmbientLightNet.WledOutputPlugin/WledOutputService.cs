using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using AmbiLightNet.PluginBase;

namespace AmbientLightNet.SerialOutputPlugin
{
    public class WledOutputService : OutputService<WledOutputInfo>
	{
        private WledOutputInfo _outputInfo;
        private UdpClient _socket;
		private readonly IDictionary<int, Color> _ledColors;

		public WledOutputService()
		{
			_ledColors = new Dictionary<int, Color>();
		}

		public override void Commit()
		{
			var maxNum = _ledColors.Keys.Max();

			var ms = new MemoryStream();
			ms.WriteByte(2); // DRGB
			ms.WriteByte(10);
			for (int i = 0; i <= maxNum; i++)
            {
				var color = _ledColors[i];
				ms.WriteByte(color.R);
				ms.WriteByte(color.G);
				ms.WriteByte(color.B);
			}

			_socket.Send(ms.ToArray(), Convert.ToInt32(ms.Length));
		}

		public override bool ColorsEqual(ColorF first, ColorF second)
		{
			return ((Color)first) == ((Color)second);
		}

		public override TimeSpan? GetResendInterval(int resendCount)
		{
			return null;
		}

		public override void Initialize(WledOutputInfo outputInfo)
		{
			if (_socket == null)
			{
				_outputInfo = outputInfo;
				_socket = new UdpClient(outputInfo.IpAddress.ToString(), 21324);
			}
		}

		public override void SetColor(ColorF color, WledOutputInfo outputInfo)
		{
			_ledColors[outputInfo.LedNumber] = (Color) color;
		}

		public override bool IsReusableFor(WledOutputInfo outputInfo)
		{
			return _outputInfo.IpAddress == outputInfo.IpAddress;
		}

		public override void Dispose()
		{
			base.Dispose();
			if (_socket != null)
			{
				_socket.Close();
				_socket = null;
			}
		}
	}
}