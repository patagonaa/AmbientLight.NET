namespace AmbientLightNet.MagicHomePlugin
{
	partial class MagicHomeOutputBroadcastConfigDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.devicesList = new System.Windows.Forms.ComboBox();
			this.devicesLabel = new System.Windows.Forms.Label();
			this.refreshButton = new System.Windows.Forms.Button();
			this.deviceTypesList = new System.Windows.Forms.ComboBox();
			this.deviceTypeLabel = new System.Windows.Forms.Label();
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// devicesList
			// 
			this.devicesList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.devicesList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.devicesList.FormattingEnabled = true;
			this.devicesList.Location = new System.Drawing.Point(8, 21);
			this.devicesList.Margin = new System.Windows.Forms.Padding(2);
			this.devicesList.Name = "devicesList";
			this.devicesList.Size = new System.Drawing.Size(364, 21);
			this.devicesList.TabIndex = 0;
			this.devicesList.SelectionChangeCommitted += new System.EventHandler(this.devicesList_SelectionChangeCommitted);
			// 
			// devicesLabel
			// 
			this.devicesLabel.AutoSize = true;
			this.devicesLabel.Location = new System.Drawing.Point(8, 6);
			this.devicesLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.devicesLabel.Name = "devicesLabel";
			this.devicesLabel.Size = new System.Drawing.Size(46, 13);
			this.devicesLabel.TabIndex = 1;
			this.devicesLabel.Text = "Devices";
			// 
			// refreshButton
			// 
			this.refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.refreshButton.Location = new System.Drawing.Point(376, 21);
			this.refreshButton.Margin = new System.Windows.Forms.Padding(2);
			this.refreshButton.Name = "refreshButton";
			this.refreshButton.Size = new System.Drawing.Size(70, 21);
			this.refreshButton.TabIndex = 2;
			this.refreshButton.Text = "Refresh";
			this.refreshButton.UseVisualStyleBackColor = true;
			this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
			// 
			// deviceTypesList
			// 
			this.deviceTypesList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.deviceTypesList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.deviceTypesList.FormattingEnabled = true;
			this.deviceTypesList.Location = new System.Drawing.Point(8, 58);
			this.deviceTypesList.Margin = new System.Windows.Forms.Padding(2);
			this.deviceTypesList.Name = "deviceTypesList";
			this.deviceTypesList.Size = new System.Drawing.Size(438, 21);
			this.deviceTypesList.TabIndex = 3;
			this.deviceTypesList.SelectionChangeCommitted += new System.EventHandler(this.deviceTypesList_SelectionChangeCommitted);
			// 
			// deviceTypeLabel
			// 
			this.deviceTypeLabel.AutoSize = true;
			this.deviceTypeLabel.Location = new System.Drawing.Point(8, 44);
			this.deviceTypeLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.deviceTypeLabel.Name = "deviceTypeLabel";
			this.deviceTypeLabel.Size = new System.Drawing.Size(68, 13);
			this.deviceTypeLabel.TabIndex = 4;
			this.deviceTypeLabel.Text = "Device Type";
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.Location = new System.Drawing.Point(378, 81);
			this.cancelButton.Margin = new System.Windows.Forms.Padding(2);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(67, 21);
			this.cancelButton.TabIndex = 5;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.Location = new System.Drawing.Point(307, 81);
			this.okButton.Margin = new System.Windows.Forms.Padding(2);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(67, 21);
			this.okButton.TabIndex = 6;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// MagicHomeOutputConfigDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(453, 110);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.deviceTypeLabel);
			this.Controls.Add(this.deviceTypesList);
			this.Controls.Add(this.refreshButton);
			this.Controls.Add(this.devicesLabel);
			this.Controls.Add(this.devicesList);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "MagicHomeOutputConfigDialog";
			this.Text = "MagicHomeOutputConfigDialog";
			this.Load += new System.EventHandler(this.MagicHomeOutputConfigDialog_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox devicesList;
		private System.Windows.Forms.Label devicesLabel;
		private System.Windows.Forms.Button refreshButton;
		private System.Windows.Forms.ComboBox deviceTypesList;
		private System.Windows.Forms.Label deviceTypeLabel;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
	}
}