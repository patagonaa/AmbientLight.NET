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
			this.autoConfigButton = new System.Windows.Forms.Button();
			this.manualConfigButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// autoConfigButton
			// 
			this.autoConfigButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.autoConfigButton.Location = new System.Drawing.Point(12, 12);
			this.autoConfigButton.Name = "autoConfigButton";
			this.autoConfigButton.Size = new System.Drawing.Size(474, 143);
			this.autoConfigButton.TabIndex = 0;
			this.autoConfigButton.Text = "Auto Config (Broadcast)";
			this.autoConfigButton.UseVisualStyleBackColor = true;
			this.autoConfigButton.Click += new System.EventHandler(this.autoConfigButton_Click);
			// 
			// manualConfigButton
			// 
			this.manualConfigButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.manualConfigButton.Location = new System.Drawing.Point(12, 161);
			this.manualConfigButton.Name = "manualConfigButton";
			this.manualConfigButton.Size = new System.Drawing.Size(474, 131);
			this.manualConfigButton.TabIndex = 1;
			this.manualConfigButton.Text = "Manual Config (IP + Port)";
			this.manualConfigButton.UseVisualStyleBackColor = true;
			this.manualConfigButton.Click += new System.EventHandler(this.manualConfigButton_Click);
			// 
			// MagicHomeOutputConfigDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(498, 304);
			this.Controls.Add(this.manualConfigButton);
			this.Controls.Add(this.autoConfigButton);
			this.Name = "MagicHomeOutputConfigDialog";
			this.Text = "MagicHomeOutputConfigDialog";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button autoConfigButton;
		private System.Windows.Forms.Button manualConfigButton;
	}
}