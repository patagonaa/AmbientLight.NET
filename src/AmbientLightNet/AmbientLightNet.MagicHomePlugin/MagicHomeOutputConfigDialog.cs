using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AmbiLightNet.PluginBase;

namespace AmbientLightNet.MagicHomePlugin
{
	public partial class MagicHomeOutputConfigDialog : OutputConfigDialog
	{
		public MagicHomeOutputConfigDialog()
		{
			InitializeComponent();
		}

		public override IOutputInfo OutputInfo { get; set; }

		private void autoConfigButton_Click(object sender, EventArgs e)
		{
			var form = new MagicHomeOutputBroadcastConfigDialog();
			var result = form.ShowDialog();

			if (result == DialogResult.OK)
			{
				DialogResult = DialogResult.OK;
				OutputInfo = form.OutputInfo;
			}

			Close();
		}

		private void manualConfigButton_Click(object sender, EventArgs e)
		{
			var form = new MagicHomeOutputManualConfigDialog();
			var result = form.ShowDialog();

			if (result == DialogResult.OK)
			{
				DialogResult = DialogResult.OK;
				OutputInfo = form.OutputInfo;
			}
			Close();
		}
	}
}
