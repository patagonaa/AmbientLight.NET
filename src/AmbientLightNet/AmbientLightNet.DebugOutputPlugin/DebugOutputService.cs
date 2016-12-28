using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using AmbiLightNet.PluginBase;

namespace AmbientLightNet.DebugOutputPlugin
{
	public class DebugOutputService : OutputService
	{
		private DebugOutputForm _form;
		private Thread _thread;

		public override void Initialize(IOutputInfo outputInfo)
		{
			_form = new DebugOutputForm();
			_form.Text = string.Format("DebugOutput {0}", ((DebugOutputInfo) outputInfo).Id);

			_thread = new Thread(() => _form.ShowDialog());
			_thread.Start();
		}

		public override void Output(Color color)
		{
			_form.SetColor(color);
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