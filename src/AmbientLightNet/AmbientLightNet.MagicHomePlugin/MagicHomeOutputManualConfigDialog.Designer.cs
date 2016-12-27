namespace AmbientLightNet.MagicHomePlugin
{
	partial class MagicHomeOutputManualConfigDialog
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
			this.deviceTypesList = new System.Windows.Forms.ComboBox();
			this.deviceTypeLabel = new System.Windows.Forms.Label();
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.ipLabel = new System.Windows.Forms.Label();
			this.ipTextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// deviceTypesList
			// 
			this.deviceTypesList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.deviceTypesList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.deviceTypesList.FormattingEnabled = true;
			this.deviceTypesList.Location = new System.Drawing.Point(13, 73);
			this.deviceTypesList.Margin = new System.Windows.Forms.Padding(2);
			this.deviceTypesList.Name = "deviceTypesList";
			this.deviceTypesList.Size = new System.Drawing.Size(547, 21);
			this.deviceTypesList.TabIndex = 3;
			this.deviceTypesList.SelectionChangeCommitted += new System.EventHandler(this.deviceTypesList_SelectionChangeCommitted);
			// 
			// deviceTypeLabel
			// 
			this.deviceTypeLabel.AutoSize = true;
			this.deviceTypeLabel.Location = new System.Drawing.Point(11, 58);
			this.deviceTypeLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.deviceTypeLabel.Name = "deviceTypeLabel";
			this.deviceTypeLabel.Size = new System.Drawing.Size(68, 13);
			this.deviceTypeLabel.TabIndex = 4;
			this.deviceTypeLabel.Text = "Device Type";
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.Location = new System.Drawing.Point(492, 96);
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
			this.okButton.Location = new System.Drawing.Point(421, 96);
			this.okButton.Margin = new System.Windows.Forms.Padding(2);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(67, 21);
			this.okButton.TabIndex = 6;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// ipLabel
			// 
			this.ipLabel.AutoSize = true;
			this.ipLabel.Location = new System.Drawing.Point(13, 13);
			this.ipLabel.Name = "ipLabel";
			this.ipLabel.Size = new System.Drawing.Size(92, 13);
			this.ipLabel.TabIndex = 7;
			this.ipLabel.Text = "IP Address ( :Port)";
			// 
			// ipTextBox
			// 
			this.ipTextBox.Location = new System.Drawing.Point(13, 30);
			this.ipTextBox.Name = "ipTextBox";
			this.ipTextBox.Size = new System.Drawing.Size(547, 20);
			this.ipTextBox.TabIndex = 8;
			// 
			// MagicHomeOutputManualConfigDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(571, 126);
			this.Controls.Add(this.ipTextBox);
			this.Controls.Add(this.ipLabel);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.deviceTypeLabel);
			this.Controls.Add(this.deviceTypesList);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "MagicHomeOutputManualConfigDialog";
			this.Text = "MagicHomeOutputConfigDialog";
			this.Load += new System.EventHandler(this.MagicHomeOutputConfigDialog_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox deviceTypesList;
		private System.Windows.Forms.Label deviceTypeLabel;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Label ipLabel;
		private System.Windows.Forms.TextBox ipTextBox;
	}
}