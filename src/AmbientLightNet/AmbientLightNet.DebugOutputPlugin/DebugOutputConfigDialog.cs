using System;
using System.Windows.Forms;
using AmbiLightNet.PluginBase;

namespace AmbientLightNet.DebugOutputPlugin
{
	public partial class DebugOutputConfigDialog : OutputConfigDialog
	{
		public DebugOutputConfigDialog()
		{
			InitializeComponent();
		}

		public override IOutputInfo OutputInfo { get; set; }

		private void cancelButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			OutputInfo = new DebugOutputInfo {Id = Guid.NewGuid()};
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
