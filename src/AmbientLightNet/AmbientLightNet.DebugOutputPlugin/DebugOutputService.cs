using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using AmbiLightNet.PluginBase;

namespace AmbientLightNet.DebugOutputPlugin
{
	public class DebugOutputService : SimpleOutputService<DebugOutputInfo>
	{
		private DebugOutputForm _form;
		private Thread _thread;

		public override void Initialize(DebugOutputInfo outputInfo)
		{
			_form = new DebugOutputForm();
			_form.Text = string.Format("DebugOutput {0}", outputInfo.Id);

			_thread = new Thread(() => _form.ShowDialog());
			_thread.Start();
		}

		protected override void Output(ColorF color)
		{
			_form.SetColor((Color) color);
		}

		public override bool ColorsEqual(ColorF first, ColorF second)
		{
			return ((Color)first) == ((Color)second);
		}

		public override TimeSpan? GetResendInterval(int resendCount)
		{
			return null;
		}

		public override void Dispose()
		{
			_form.Invoke((MethodInvoker) delegate
			{
				_form.Close();
				_form.Dispose();
			});
			
			_thread.Abort();
			
			base.Dispose();
		}
	}
}