using System;
using System.Windows.Forms;

namespace AmbiLightNet.PluginBase
{
	public class OutputConfigDialog : Form
	{
		public virtual IOutputInfo OutputInfo
		{
			get { Console.WriteLine("???"); return null; }
			set { Console.WriteLine("???"); }
		}
	}
}