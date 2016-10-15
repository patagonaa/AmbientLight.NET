using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AmbiLightNet.PluginBase;
using MagicHomeController;

namespace AmbientLightNet.MagicHomePlugin
{
	public partial class MagicHomeOutputConfigDialog : OutputConfigDialog
	{
		public MagicHomeOutputConfigDialog()
		{
			InitializeComponent();
			RefreshDevicesList();

			deviceTypesList.DisplayMember = "Value";
			deviceTypesList.Items.AddRange(
				Enum.GetValues(typeof (DeviceType))
					.Cast<DeviceType>()
					.ToDictionary(x => x, x => Enum.GetName(typeof (DeviceType), x))
					.Cast<object>()
					.ToArray()
				);
		}

		private void RefreshDevicesList()
		{
			IEnumerable<DeviceFindResult> devices = DeviceFinder.FindDevices();

			var oldSelected = devicesList.SelectedItem as DeviceFindResult;

			devicesList.Items.Clear();
			devicesList.Items.AddRange(devices.Cast<object>().ToArray());

			if (oldSelected != null)
			{
				int itemIndex = devicesList.Items
					.Cast<DeviceFindResult>()
					.ToList()
					.FindIndex(x => x.MacAddress.Equals(oldSelected.MacAddress));
				if (itemIndex != -1)
				{
					devicesList.SelectedIndex = itemIndex;
					return;
				}
			}

			if (devicesList.Items.Count > 0)
			{
				devicesList.SelectedIndex = 0;
			}
		}

		public override IOutputInfo OutputInfo
		{
			get
			{
				if (DialogResult == DialogResult.OK)
				{
					return new MagicHomeLedOutput
					{
						MacAddress = ((DeviceFindResult) devicesList.SelectedItem).MacAddress,
						DeviceType = ((KeyValuePair<DeviceType, string>)deviceTypesList.SelectedItem).Key
					};
				}
				return null;
			}
			set
			{
				
			}
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void refreshButton_Click(object sender, EventArgs e)
		{
			RefreshDevicesList();
		}
	}
}
