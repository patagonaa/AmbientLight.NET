namespace AmbientLightNet.Configurator
{
	partial class ConfigForm
	{
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Vom Windows Form-Designer generierter Code

		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung.
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.screenPreview = new System.Windows.Forms.PictureBox();
			this.screenList = new System.Windows.Forms.ComboBox();
			this.loadConfigButton = new System.Windows.Forms.Button();
			this.screenRegionsList = new System.Windows.Forms.ListBox();
			this.removeRegionButton = new System.Windows.Forms.Button();
			this.saveConfigButton = new System.Windows.Forms.Button();
			this.setTopLeftButton = new System.Windows.Forms.Button();
			this.setBottomRightButton = new System.Windows.Forms.Button();
			this.addRegionButton = new System.Windows.Forms.Button();
			this.editOutputModeButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.screenPreview)).BeginInit();
			this.SuspendLayout();
			// 
			// screenPreview
			// 
			this.screenPreview.Location = new System.Drawing.Point(12, 49);
			this.screenPreview.Name = "screenPreview";
			this.screenPreview.Size = new System.Drawing.Size(1280, 720);
			this.screenPreview.TabIndex = 0;
			this.screenPreview.TabStop = false;
			this.screenPreview.Click += new System.EventHandler(this.screenPreview_Click);
			this.screenPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.screenPreview_Paint);
			// 
			// screenList
			// 
			this.screenList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.screenList.FormattingEnabled = true;
			this.screenList.Location = new System.Drawing.Point(284, 14);
			this.screenList.Name = "screenList";
			this.screenList.Size = new System.Drawing.Size(438, 28);
			this.screenList.TabIndex = 1;
			this.screenList.SelectionChangeCommitted += new System.EventHandler(this.screenList_SelectionChangeCommitted);
			// 
			// loadConfigButton
			// 
			this.loadConfigButton.Location = new System.Drawing.Point(12, 12);
			this.loadConfigButton.Name = "loadConfigButton";
			this.loadConfigButton.Size = new System.Drawing.Size(130, 30);
			this.loadConfigButton.TabIndex = 2;
			this.loadConfigButton.Text = "Load Config";
			this.loadConfigButton.UseVisualStyleBackColor = true;
			this.loadConfigButton.Click += new System.EventHandler(this.loadConfigButton_Click);
			// 
			// screenRegionsList
			// 
			this.screenRegionsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.screenRegionsList.FormattingEnabled = true;
			this.screenRegionsList.ItemHeight = 20;
			this.screenRegionsList.Location = new System.Drawing.Point(1298, 46);
			this.screenRegionsList.Name = "screenRegionsList";
			this.screenRegionsList.Size = new System.Drawing.Size(408, 544);
			this.screenRegionsList.TabIndex = 3;
			// 
			// removeRegionButton
			// 
			this.removeRegionButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.removeRegionButton.Location = new System.Drawing.Point(1298, 637);
			this.removeRegionButton.Name = "removeRegionButton";
			this.removeRegionButton.Size = new System.Drawing.Size(408, 30);
			this.removeRegionButton.TabIndex = 5;
			this.removeRegionButton.Text = "-";
			this.removeRegionButton.UseVisualStyleBackColor = true;
			this.removeRegionButton.Click += new System.EventHandler(this.removeRegionButton_Click);
			// 
			// saveConfigButton
			// 
			this.saveConfigButton.Location = new System.Drawing.Point(148, 13);
			this.saveConfigButton.Name = "saveConfigButton";
			this.saveConfigButton.Size = new System.Drawing.Size(130, 30);
			this.saveConfigButton.TabIndex = 6;
			this.saveConfigButton.Text = "Save Config";
			this.saveConfigButton.UseVisualStyleBackColor = true;
			this.saveConfigButton.Click += new System.EventHandler(this.saveConfigButton_Click);
			// 
			// setTopLeftButton
			// 
			this.setTopLeftButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.setTopLeftButton.Location = new System.Drawing.Point(1298, 673);
			this.setTopLeftButton.Name = "setTopLeftButton";
			this.setTopLeftButton.Size = new System.Drawing.Size(408, 30);
			this.setTopLeftButton.TabIndex = 7;
			this.setTopLeftButton.Text = "Top L";
			this.setTopLeftButton.UseVisualStyleBackColor = true;
			this.setTopLeftButton.Click += new System.EventHandler(this.setTopLeftButton_Click);
			// 
			// setBottomRightButton
			// 
			this.setBottomRightButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.setBottomRightButton.Location = new System.Drawing.Point(1298, 709);
			this.setBottomRightButton.Name = "setBottomRightButton";
			this.setBottomRightButton.Size = new System.Drawing.Size(408, 30);
			this.setBottomRightButton.TabIndex = 8;
			this.setBottomRightButton.Text = "Bottom R";
			this.setBottomRightButton.UseVisualStyleBackColor = true;
			this.setBottomRightButton.Click += new System.EventHandler(this.setBottomRightButton_Click);
			// 
			// addRegionButton
			// 
			this.addRegionButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.addRegionButton.Location = new System.Drawing.Point(1298, 601);
			this.addRegionButton.Name = "addRegionButton";
			this.addRegionButton.Size = new System.Drawing.Size(408, 30);
			this.addRegionButton.TabIndex = 9;
			this.addRegionButton.Text = "+";
			this.addRegionButton.UseVisualStyleBackColor = true;
			this.addRegionButton.Click += new System.EventHandler(this.addRegionButton_Click);
			// 
			// editOutputModeButton
			// 
			this.editOutputModeButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.editOutputModeButton.Location = new System.Drawing.Point(1298, 745);
			this.editOutputModeButton.Name = "editOutputModeButton";
			this.editOutputModeButton.Size = new System.Drawing.Size(408, 30);
			this.editOutputModeButton.TabIndex = 10;
			this.editOutputModeButton.Text = "Edit Output Mode";
			this.editOutputModeButton.UseVisualStyleBackColor = true;
			this.editOutputModeButton.Click += new System.EventHandler(this.editOutputModeButton_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1718, 785);
			this.Controls.Add(this.editOutputModeButton);
			this.Controls.Add(this.addRegionButton);
			this.Controls.Add(this.setBottomRightButton);
			this.Controls.Add(this.setTopLeftButton);
			this.Controls.Add(this.saveConfigButton);
			this.Controls.Add(this.removeRegionButton);
			this.Controls.Add(this.screenRegionsList);
			this.Controls.Add(this.loadConfigButton);
			this.Controls.Add(this.screenList);
			this.Controls.Add(this.screenPreview);
			this.MaximizeBox = false;
			this.Name = "Form1";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Form1";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			((System.ComponentModel.ISupportInitialize)(this.screenPreview)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox screenPreview;
		private System.Windows.Forms.ComboBox screenList;
		private System.Windows.Forms.Button loadConfigButton;
		private System.Windows.Forms.ListBox screenRegionsList;
		private System.Windows.Forms.Button removeRegionButton;
		private System.Windows.Forms.Button saveConfigButton;
		private System.Windows.Forms.Button setTopLeftButton;
		private System.Windows.Forms.Button setBottomRightButton;
		private System.Windows.Forms.Button addRegionButton;
		private System.Windows.Forms.Button editOutputModeButton;
	}
}

