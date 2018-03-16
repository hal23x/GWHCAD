namespace GWHCAD
{
	partial class PrintOutputForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintOutputForm));
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.lTSUpdateBtn = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.lTSPrintBtn = new System.Windows.Forms.ToolStripButton();
			this.lTSPrintPreviewBtn = new System.Windows.Forms.ToolStripButton();
			this.lTSPrintSettingsBtn = new System.Windows.Forms.ToolStripButton();
			this.lDesignOutputCtrl = new GWHCAD.DesignOutputControl();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lTSUpdateBtn,
            this.toolStripSeparator1,
            this.lTSPrintBtn,
            this.lTSPrintPreviewBtn,
            this.lTSPrintSettingsBtn});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(570, 25);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// lTSUpdateBtn
			// 
			this.lTSUpdateBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.lTSUpdateBtn.Image = ((System.Drawing.Image)(resources.GetObject("lTSUpdateBtn.Image")));
			this.lTSUpdateBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.lTSUpdateBtn.Name = "lTSUpdateBtn";
			this.lTSUpdateBtn.Size = new System.Drawing.Size(23, 22);
			this.lTSUpdateBtn.Text = "Update Document";
			this.lTSUpdateBtn.Click += new System.EventHandler(this.lTSUpdateBtn_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// lTSPrintBtn
			// 
			this.lTSPrintBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.lTSPrintBtn.Image = ((System.Drawing.Image)(resources.GetObject("lTSPrintBtn.Image")));
			this.lTSPrintBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.lTSPrintBtn.Name = "lTSPrintBtn";
			this.lTSPrintBtn.Size = new System.Drawing.Size(23, 22);
			this.lTSPrintBtn.Text = "Print";
			this.lTSPrintBtn.Click += new System.EventHandler(this.lTSPrintBtn_Click);
			// 
			// lTSPrintPreviewBtn
			// 
			this.lTSPrintPreviewBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.lTSPrintPreviewBtn.Image = ((System.Drawing.Image)(resources.GetObject("lTSPrintPreviewBtn.Image")));
			this.lTSPrintPreviewBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.lTSPrintPreviewBtn.Name = "lTSPrintPreviewBtn";
			this.lTSPrintPreviewBtn.Size = new System.Drawing.Size(23, 22);
			this.lTSPrintPreviewBtn.Text = "Print Preview";
			// 
			// lTSPrintSettingsBtn
			// 
			this.lTSPrintSettingsBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.lTSPrintSettingsBtn.Image = ((System.Drawing.Image)(resources.GetObject("lTSPrintSettingsBtn.Image")));
			this.lTSPrintSettingsBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.lTSPrintSettingsBtn.Name = "lTSPrintSettingsBtn";
			this.lTSPrintSettingsBtn.Size = new System.Drawing.Size(23, 22);
			this.lTSPrintSettingsBtn.Text = "Printer Settings";
			// 
			// lDesignOutputCtrl
			// 
			this.lDesignOutputCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lDesignOutputCtrl.BackColor = System.Drawing.Color.White;
			this.lDesignOutputCtrl.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.lDesignOutputCtrl.Location = new System.Drawing.Point(12, 29);
			this.lDesignOutputCtrl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.lDesignOutputCtrl.Name = "lDesignOutputCtrl";
			this.lDesignOutputCtrl.Size = new System.Drawing.Size(546, 379);
			this.lDesignOutputCtrl.TabIndex = 0;
			// 
			// PrintOutputForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(570, 421);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.lDesignOutputCtrl);
			this.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.Name = "PrintOutputForm";
			this.Text = "PrintOutputForm";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PrintOutputForm_FormClosing);
			this.Move += new System.EventHandler(this.PrintOutputForm_Move);
			this.Resize += new System.EventHandler(this.PrintOutputForm_Resize);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private DesignOutputControl lDesignOutputCtrl;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton lTSUpdateBtn;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton lTSPrintBtn;
		private System.Windows.Forms.ToolStripButton lTSPrintPreviewBtn;
		private System.Windows.Forms.ToolStripButton lTSPrintSettingsBtn;
	}
}