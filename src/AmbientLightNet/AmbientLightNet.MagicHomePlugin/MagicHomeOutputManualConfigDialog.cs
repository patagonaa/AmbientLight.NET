using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Forms;
using AmbiLightNet.PluginBase;
using MagicHomeController;
using System.Net;
using System.Globalization;

namespace AmbientLightNet.MagicHomePlugin
{
	public partial class MagicHomeOutputManualConfigDialog : Form
	{
		private DeviceType _selectedType;

		public MagicHomeOutputManualConfigDialog()
		{
			InitializeComponent();

			deviceTypesList.DisplayMember = "Value";

			deviceTypesList.Items.AddRange(
				Enum.GetValues(typeof (DeviceType))
					.Cast<DeviceType>()
					.ToDictionary(x => x, x => Enum.GetName(typeof (DeviceType), x))
					.Cast<object>()
					.ToArray()
				);

			_selectedType = DeviceType.RgbWarmwhite;
		}

		private void RefreshTypesList()
		{
			int idx = deviceTypesList.Items
				.Cast<KeyValuePair<DeviceType, string>>()
				.ToList()
				.FindIndex(x => x.Key == _selectedType);

			deviceTypesList.SelectedIndex = idx == -1 ? 0 : idx;
		}

		public IOutputInfo OutputInfo
		{
			get
			{
				if (DialogResult == DialogResult.OK)
				{
					var split = ipTextBox.Text.Split(':');

					IPAddress ip = IPAddress.Parse(split[0]);
					int? port = null;

					if (split.Length == 2)
					{
						port = int.Parse(split[1], CultureInfo.InvariantCulture);
					}

					return new MagicHomeLedOutput
					{
						AddressType = AddressType.IpAddress,
						DeviceType = ((KeyValuePair<DeviceType, string>)deviceTypesList.SelectedItem).Key,
						IPAddress = ip,
						Port = port
					};
				}
				return null;
			}
			set
			{
				var magicHomeOutputInfo = value as MagicHomeLedOutput;
				if(magicHomeOutputInfo == null)
					return;
				_selectedType = magicHomeOutputInfo.DeviceType;
				ipTextBox.Text = magicHomeOutputInfo.AddressType == AddressType.IpAddress
					? (magicHomeOutputInfo.Port.HasValue
						? magicHomeOutputInfo.IPAddress.ToString() + ":" + magicHomeOutputInfo.Port.Value.ToString()
						: magicHomeOutputInfo.IPAddress.ToString())
					: "";
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

		private void deviceTypesList_SelectionChangeCommitted(object sender, EventArgs e)
		{
			var selectedDeviceType = (KeyValuePair<DeviceType, string>)deviceTypesList.SelectedItem;

			_selectedType = selectedDeviceType.Key;
		}

		private void MagicHomeOutputConfigDialog_Load(object sender, EventArgs e)
		{
			RefreshTypesList();
		}
	}
}
