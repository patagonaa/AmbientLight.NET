using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AmbientLightNet.Infrastructure.AmbiLightConfig;
using AmbientLightNet.Infrastructure.ScreenCapture;
using AmbientLightNet.ScreenCapture.Infrastructure;
using Newtonsoft.Json;

namespace AmbientLightNet.Configurator
{
	public partial class ConfigForm : Form
	{
		private readonly Timer _updateImageTimer;
		private readonly IScreenCaptureService _captureService;
		private Screen _selectedScreen;
		private Bitmap _lastBitmap;

		public ConfigForm()
		{
			InitializeComponent();

			var captureServiceProvider = new ScreenCaptureServiceProvider();

			_captureService = captureServiceProvider.Provide(false);

			_updateImageTimer = new Timer();
			_updateImageTimer.Interval = 100;
			_updateImageTimer.Tick += UpdatePictureBox;
			_updateImageTimer.Start();

			screenList.DataSource = Screen.AllScreens;
			screenList.DisplayMember = "DeviceName";
			Screen primaryScreen = Screen.PrimaryScreen;
			screenList.SelectedItem = primaryScreen;
			_selectedScreen = primaryScreen;

			SetScreenClickMode(ScreenClickMode.None);
		}

		private void UpdatePictureBox(object sender, EventArgs e)
		{
			screenPreview.Invalidate();
		}

		private void screenPreview_Paint(object sender, PaintEventArgs e)
		{
			if (_selectedScreen == null)
				return;

			string screenName = _selectedScreen.DeviceName;

			var region = new ScreenRegion { ScreenName = screenName, Rectangle = new RectangleF(0, 0, 1, 1) };
			Bitmap imageToShow = _captureService.CaptureScreenRegions(new List<ScreenRegion> {region})[0];

			if (imageToShow == null)
			{
				if (_lastBitmap == null)
					return;
				imageToShow = _lastBitmap;
			}
			else
			{
				if (_lastBitmap != null)
					_lastBitmap.Dispose();
				_lastBitmap = imageToShow;
			}

			e.Graphics.DrawImage(imageToShow, 0, 0, e.ClipRectangle.Width, e.ClipRectangle.Height);

			foreach (ScreenRegionOutput screenRegionOutput in screenRegionsList.Items.Cast<ScreenRegionOutput>().Where(x => x.ScreenRegion.ScreenName == screenName))
			{
				RectangleF screenRegionRect = screenRegionOutput.ScreenRegion.Rectangle;
				e.Graphics.DrawRectangle(Pens.Red,
					screenRegionRect.X*e.ClipRectangle.Width,
					screenRegionRect.Y*e.ClipRectangle.Height,
					screenRegionRect.Width*e.ClipRectangle.Width,
					screenRegionRect.Height*e.ClipRectangle.Height);
			}
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			_updateImageTimer.Dispose();
			_captureService.Dispose();
		}

		private void screenList_SelectionChangeCommitted(object sender, EventArgs e)
		{
			_selectedScreen = screenList.SelectedValue as Screen;
		}

		private void loadConfigButton_Click(object sender, EventArgs e)
		{
			var dialog = new OpenFileDialog();
			dialog.Filter = "JSON (*.json)|*.json|All files (*.*)|*.*";

			DialogResult result = dialog.ShowDialog();

			if (result != DialogResult.OK) 
				return;

			using (Stream stream = dialog.OpenFile())
			{
				using (var sr = new StreamReader(stream, Encoding.UTF8))
				{
					LoadConfig(sr.ReadToEnd());
				}
			}
		}

		private void LoadConfig(string str)
		{
			var config = JsonConvert.DeserializeObject<AmbiLightConfig>(str);

			screenRegionsList.Items.Clear();
			screenRegionsList.Items.AddRange(config.RegionsToOutput.Cast<object>().ToArray());
		}

		private void saveConfigButton_Click(object sender, EventArgs e)
		{
			var dialog = new SaveFileDialog();
			dialog.Filter = "JSON (*.json)|*.json|All files (*.*)|*.*";

			DialogResult result = dialog.ShowDialog();

			if (result != DialogResult.OK)
				return;

			using (Stream stream = dialog.OpenFile())
			{
				stream.SetLength(0);

				using (var sw = new StreamWriter(stream, Encoding.UTF8))
				{
					sw.Write(SaveConfig());
				}
			}
		}

		private string SaveConfig()
		{
			var config = new AmbiLightConfig();

			config.RegionsToOutput = screenRegionsList.Items.Cast<ScreenRegionOutput>().ToList();

			return JsonConvert.SerializeObject(config, Formatting.Indented);
		}

		private void SetScreenClickMode(ScreenClickMode mode)
		{
			switch (mode)
			{
				case ScreenClickMode.None:
					setTopLeftButton.Text = "Top Left";
					setBottomRightButton.Text = "Bottom Right";
					break;
				case ScreenClickMode.SetTopLeft:
					setTopLeftButton.Text = "...";
					break;
				case ScreenClickMode.SetBottomRight:
					setBottomRightButton.Text = "...";
					break;
				default:
					throw new ArgumentOutOfRangeException("mode", mode, null);
			}

			_screenClickMode = mode;
		}

		private ScreenClickMode _screenClickMode;

		private void screenPreview_Click(object sender, EventArgs e)
		{
			var mouseArgs = e as MouseEventArgs;

			if (mouseArgs == null || mouseArgs.Button != MouseButtons.Left)
				return;

			float posX = (float) mouseArgs.X/screenPreview.Width;
			float posY = (float) mouseArgs.Y/screenPreview.Height;

			switch (_screenClickMode)
			{
				case ScreenClickMode.None:
					return;
				case ScreenClickMode.SetTopLeft:
					var region = screenRegionsList.SelectedItem as ScreenRegionOutput;

					if (region == null)
						break;

					region.ScreenRegion.ScreenName = _selectedScreen.DeviceName;

					RectangleF rect = region.ScreenRegion.Rectangle;
					SetRegion(ref rect, new PointF(posX, posY), null);
					region.ScreenRegion.Rectangle = rect;
					break;
				case ScreenClickMode.SetBottomRight:
					var region2 = screenRegionsList.SelectedItem as ScreenRegionOutput;

					if (region2 == null)
						break;

					region2.ScreenRegion.ScreenName = _selectedScreen.DeviceName;

					RectangleF rect2 = region2.ScreenRegion.Rectangle;
					SetRegion(ref rect2, null, new PointF(posX, posY));
					region2.ScreenRegion.Rectangle = rect2;

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			UpdateSelectedItem(screenRegionsList);
			SetScreenClickMode(ScreenClickMode.None);
		}

		private static void UpdateSelectedItem(ListBox regionsList)
		{
			var idx = regionsList.SelectedIndex;
			if(idx == -1)
				return;

			var obj = regionsList.Items[idx];
			regionsList.Items.RemoveAt(idx);
			regionsList.Items.Insert(idx, obj);

			regionsList.SelectedIndex = idx;
		}

		private static void SetRegion(ref RectangleF rectangle, PointF? topLeft, PointF? bottomRight)
		{
			PointF topLeftValue = topLeft ?? rectangle.Location;
			PointF bottomRightValue = bottomRight ?? PointF.Add(rectangle.Location, rectangle.Size);

			if (topLeftValue.X > bottomRightValue.X)
			{
				bottomRightValue.X = topLeftValue.X;
			}
			if (topLeftValue.Y > bottomRightValue.Y)
			{
				bottomRightValue.Y = topLeftValue.Y;
			}

			rectangle.X = topLeftValue.X;
			rectangle.Y = topLeftValue.Y;

			rectangle.Width = bottomRightValue.X - topLeftValue.X;
			rectangle.Height = bottomRightValue.Y - topLeftValue.Y;
		}

		private void setTopLeftButton_Click(object sender, EventArgs e)
		{
			SetScreenClickMode(ScreenClickMode.SetTopLeft);
		}

		private void setBottomRightButton_Click(object sender, EventArgs e)
		{
			SetScreenClickMode(ScreenClickMode.SetBottomRight);
		}

		private void removeRegionButton_Click(object sender, EventArgs e)
		{
			if (screenRegionsList.SelectedIndex == -1)
				return;

			screenRegionsList.Items.RemoveAt(screenRegionsList.SelectedIndex);
		}

		private void addRegionButton_Click(object sender, EventArgs e)
		{
			screenRegionsList.Items.Add(new ScreenRegionOutput
			{
				ScreenRegion = new ScreenRegion
				{
					Rectangle = new RectangleF(0, 0, 0, 0),
					ScreenName = _selectedScreen.DeviceName
				},
				Outputs = null
			});

			screenRegionsList.SelectedIndex = screenRegionsList.Items.Count - 1;
		}

		private enum ScreenClickMode
		{
			None,
			SetTopLeft,
			SetBottomRight
		}

		private void editOutputModeButton_Click(object sender, EventArgs e)
		{
			if (screenRegionsList.SelectedIndex == -1)
				return;

			var changeOutputDialog = new ChangeOutputForm();

			var screenRegionOutput = (ScreenRegionOutput) screenRegionsList.Items[screenRegionsList.SelectedIndex];

			changeOutputDialog.OutputInfo = screenRegionOutput.Outputs != null && screenRegionOutput.Outputs.Count > 0 ? screenRegionOutput.Outputs[0].OutputInfo : null;

			DialogResult result = changeOutputDialog.ShowDialog();

			if (result != DialogResult.OK)
				return;

			screenRegionOutput.Outputs = new[] { new Output { OutputInfo = changeOutputDialog.OutputInfo } };

			UpdateSelectedItem(screenRegionsList);
		}
	}


}
