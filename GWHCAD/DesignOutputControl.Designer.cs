namespace GWHCAD
{
	partial class DesignOutputControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lHScrollBar = new System.Windows.Forms.HScrollBar();
			this.lVScrollBar = new System.Windows.Forms.VScrollBar();
			this.SuspendLayout();
			// 
			// lHScrollBar
			// 
			this.lHScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lHScrollBar.Location = new System.Drawing.Point(0, 349);
			this.lHScrollBar.Name = "lHScrollBar";
			this.lHScrollBar.Size = new System.Drawing.Size(397, 16);
			this.lHScrollBar.TabIndex = 0;
			this.lHScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.lHScrollBar_Scroll);
			// 
			// lVScrollBar
			// 
			this.lVScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lVScrollBar.Location = new System.Drawing.Point(397, 0);
			this.lVScrollBar.Name = "lVScrollBar";
			this.lVScrollBar.Size = new System.Drawing.Size(16, 349);
			this.lVScrollBar.TabIndex = 1;
			this.lVScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.lVScrollBar_Scroll);
			// 
			// DesignOutputControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.lVScrollBar);
			this.Controls.Add(this.lHScrollBar);
			this.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "DesignOutputControl";
			this.Size = new System.Drawing.Size(413, 365);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.DesignOutputControl_Paint);
			this.Resize += new System.EventHandler(this.DesignOutputControl_Resize);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.HScrollBar lHScrollBar;
		private System.Windows.Forms.VScrollBar lVScrollBar;
	}
}
