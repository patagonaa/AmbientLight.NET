using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AmbientLightNet.Infrastructure;
using AmbiLightNet.PluginBase;

namespace AmbientLightNet.Configurator
{
	public partial class ChangeOutputForm : Form
	{
		private IOutputInfo _outputInfo;
		private List<IOutputPlugin> _plugins;

		public ChangeOutputForm()
		{
			InitializeComponent();

			_plugins = OutputPlugins.GetAvailablePlugins();

			outputPluginsDropDown.DisplayMember = "DisplayName";

			if (_plugins.Count == 0)
			{
				okButton.Enabled = false;
			}
			else
			{
				outputPluginsDropDown.Items.AddRange(_plugins.Cast<object>().ToArray());
				outputPluginsDropDown.SelectedIndex = 0;
			}
		}

		public IOutputInfo OutputInfo
		{
			get { return _outputInfo; }
			set
			{
				_outputInfo = value;
				if(_outputInfo == null)
					return;

				int pluginIndex = _plugins.FindIndex(x => x.Name == OutputInfo.PluginName);
				if (pluginIndex != -1)
					outputPluginsDropDown.SelectedIndex = pluginIndex;
			}
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			var plugin = (IOutputPlugin) outputPluginsDropDown.SelectedItem;

			var dialog = plugin.GetOutputConfigDialog();

			dialog.ShowDialog();

			if (dialog.DialogResult != DialogResult.OK)
			{
				DialogResult = DialogResult.Cancel;
				_outputInfo = null;
				return;
			}

			_outputInfo = dialog.OutputInfo;

			DialogResult = DialogResult.OK;
			Close();
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
