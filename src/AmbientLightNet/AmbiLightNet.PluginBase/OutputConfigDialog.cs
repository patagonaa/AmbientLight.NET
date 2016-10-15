using System;
using System.Windows.Forms;

namespace AmbiLightNet.PluginBase
{
	public class OutputConfigDialog : Form
	{
		public virtual IOutputInfo OutputInfo
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
	}
}