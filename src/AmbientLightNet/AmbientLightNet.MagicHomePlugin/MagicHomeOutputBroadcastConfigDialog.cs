using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Forms;
using AmbiLightNet.PluginBase;
using MagicHomeController;

namespace AmbientLightNet.MagicHomePlugin
{
	public partial class MagicHomeOutputBroadcastConfigDialog : Form
	{
		private DeviceType _selectedType;
		private PhysicalAddress _selectedDevice;

		public MagicHomeOutputBroadcastConfigDialog()
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

			_selectedType = DeviceType.RgbWarmwhite;
			_selectedDevice = null;
		}

		private void RefreshTypesList()
		{
			int idx = deviceTypesList.Items
				.Cast<KeyValuePair<DeviceType, string>>()
				.ToList()
				.FindIndex(x => x.Key == _selectedType);

			deviceTypesList.SelectedIndex = idx == -1 ? 0 : idx;
		}

		private void RefreshDevicesList()
		{
			var task = new Task<IEnumerable<DeviceFindResult>>(() => DeviceFinder.FindDevices().ToList());
			task.Start();
			refreshButton.Text = "...";
			task.ContinueWith(tsk => this.Invoke((MethodInvoker) (() => RefreshListDone(tsk.Result))));
		}

		private void RefreshListDone(IEnumerable<DeviceFindResult> devices)
		{
			refreshButton.Text = "Refresh";

			devicesList.Items.Clear();
			devicesList.Items.AddRange(devices.Cast<object>().ToArray());

			if (_selectedDevice != null)
			{
				int itemIndex = devicesList.Items
					.Cast<DeviceFindResult>()
					.ToList()
					.FindIndex(x => x.MacAddress.Equals(_selectedDevice));
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

		public IOutputInfo OutputInfo
		{
			get
			{
				if (DialogResult == DialogResult.OK)
				{
					return new MagicHomeLedOutputInfo
					{
						MacAddress = ((DeviceFindResult) devicesList.SelectedItem).MacAddress,
						DeviceType = ((KeyValuePair<DeviceType, string>)deviceTypesList.SelectedItem).Key,
						AddressType = AddressType.MacAddress
					};
				}
				return null;
			}
			set
			{
				var magicHomeOutputInfo = value as MagicHomeLedOutputInfo;
				if(magicHomeOutputInfo == null)
					return;
				_selectedType = magicHomeOutputInfo.DeviceType;
				_selectedDevice = magicHomeOutputInfo.MacAddress;
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

		private void devicesList_SelectionChangeCommitted(object sender, EventArgs e)
		{
			var selectedDevice = devicesList.SelectedItem as DeviceFindResult;

			_selectedDevice = selectedDevice == null ? null : selectedDevice.MacAddress;
		}

		private void deviceTypesList_SelectionChangeCommitted(object sender, EventArgs e)
		{
			var selectedDeviceType = (KeyValuePair<DeviceType, string>)deviceTypesList.SelectedItem;

			_selectedType = selectedDeviceType.Key;
		}

		private void MagicHomeOutputConfigDialog_Load(object sender, EventArgs e)
		{
			RefreshDevicesList();
			RefreshTypesList();
		}
	}
}
