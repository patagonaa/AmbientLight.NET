namespace AmbientLightNet.Configurator
{
	partial class ChangeOutputForm
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
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.outputPluginsDropDown = new System.Windows.Forms.ComboBox();
			this.outputPluginsLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// cancelButton
			// 
			this.cancelButton.Location = new System.Drawing.Point(681, 66);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(120, 30);
			this.cancelButton.TabIndex = 0;
			this.cancelButton.Text = "Abbrechen";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// okButton
			// 
			this.okButton.Location = new System.Drawing.Point(555, 66);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(120, 30);
			this.okButton.TabIndex = 1;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// outputPluginsDropDown
			// 
			this.outputPluginsDropDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.outputPluginsDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.outputPluginsDropDown.FormattingEnabled = true;
			this.outputPluginsDropDown.Location = new System.Drawing.Point(12, 32);
			this.outputPluginsDropDown.Name = "outputPluginsDropDown";
			this.outputPluginsDropDown.Size = new System.Drawing.Size(789, 28);
			this.outputPluginsDropDown.TabIndex = 2;
			// 
			// outputPluginsLabel
			// 
			this.outputPluginsLabel.AutoSize = true;
			this.outputPluginsLabel.Location = new System.Drawing.Point(12, 9);
			this.outputPluginsLabel.Name = "outputPluginsLabel";
			this.outputPluginsLabel.Size = new System.Drawing.Size(112, 20);
			this.outputPluginsLabel.TabIndex = 3;
			this.outputPluginsLabel.Text = "Output plugins";
			// 
			// AddScreenRegionOutputForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(813, 112);
			this.Controls.Add(this.outputPluginsLabel);
			this.Controls.Add(this.outputPluginsDropDown);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.cancelButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "AddScreenRegionOutputForm";
			this.Text = "AddScreenRegionOutputForm";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.ComboBox outputPluginsDropDown;
		private System.Windows.Forms.Label outputPluginsLabel;
	}
}