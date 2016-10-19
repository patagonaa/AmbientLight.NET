using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AmbientLightNet.DebugOutputPlugin
{
	public partial class DebugOutputForm : Form
	{
		private Color _color;

		public DebugOutputForm()
		{
			InitializeComponent();
		}

		public void SetColor(Color c)
		{
			_color = c;
			Invalidate();
		}

		private void DebugOutputForm_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.FillRectangle(new SolidBrush(_color), e.Graphics.ClipBounds);
		}
	}
}
