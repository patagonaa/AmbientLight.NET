namespace AmbientLightNet.MagicHomePlugin
{
	partial class MagicHomeOutputConfigDialog
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
			this.devicesList.Location = new System.Drawing.Point(12, 32);
			this.devicesList.Name = "devicesList";
			this.devicesList.Size = new System.Drawing.Size(544, 28);
			this.devicesList.TabIndex = 0;
			// 
			// devicesLabel
			// 
			this.devicesLabel.AutoSize = true;
			this.devicesLabel.Location = new System.Drawing.Point(12, 9);
			this.devicesLabel.Name = "devicesLabel";
			this.devicesLabel.Size = new System.Drawing.Size(65, 20);
			this.devicesLabel.TabIndex = 1;
			this.devicesLabel.Text = "Devices";
			// 
			// refreshButton
			// 
			this.refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.refreshButton.Location = new System.Drawing.Point(562, 32);
			this.refreshButton.Name = "refreshButton";
			this.refreshButton.Size = new System.Drawing.Size(105, 28);
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
			this.deviceTypesList.Location = new System.Drawing.Point(12, 90);
			this.deviceTypesList.Name = "deviceTypesList";
			this.deviceTypesList.Size = new System.Drawing.Size(655, 28);
			this.deviceTypesList.TabIndex = 3;
			// 
			// deviceTypeLabel
			// 
			this.deviceTypeLabel.AutoSize = true;
			this.deviceTypeLabel.Location = new System.Drawing.Point(12, 67);
			this.deviceTypeLabel.Name = "deviceTypeLabel";
			this.deviceTypeLabel.Size = new System.Drawing.Size(95, 20);
			this.deviceTypeLabel.TabIndex = 4;
			this.deviceTypeLabel.Text = "Device Type";
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.Location = new System.Drawing.Point(567, 124);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(100, 30);
			this.cancelButton.TabIndex = 5;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.Location = new System.Drawing.Point(461, 124);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(100, 30);
			this.okButton.TabIndex = 6;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// MagicHomeOutputConfigDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(679, 169);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.deviceTypeLabel);
			this.Controls.Add(this.deviceTypesList);
			this.Controls.Add(this.refreshButton);
			this.Controls.Add(this.devicesLabel);
			this.Controls.Add(this.devicesList);
			this.Name = "MagicHomeOutputConfigDialog";
			this.Text = "MagicHomeOutputConfigDialog";
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