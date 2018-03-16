using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GWHCAD
{
	public partial class DesignObject3DControl : UserControl
	{
		public double Theta
		{
			get { return lDOGraphics.Theta; }
			set { lDOGraphics.Theta = value; ForceRedraw(); }
		}
		public double Phi
		{
			get { return lDOGraphics.Phi; }
			set { lDOGraphics.Phi = value; ForceRedraw(); }
		}
		public double Scaling
		{
			get { return lDOGraphics.Scaling; }
			set { lDOGraphics.Scaling = value; ForceRedraw(); }
		}
		public int MidpointX
		{
			get { return lDOGraphics.MidpointX; }
			set { lDOGraphics.MidpointX = value; ForceRedraw(); }
		}
		public int MidpointY
		{
			get { return lDOGraphics.MidpointY; }
			set { lDOGraphics.MidpointY = value; ForceRedraw(); }
		}
		public bool ShowAxes
		{
			get { return lDOGraphics.ShowAxes; }
			set { lDOGraphics.ShowAxes = value; ForceRedraw(); }
		}
		public bool ShowLabels
		{
			get { return lDOGraphics.ShowLabels; }
			set { lDOGraphics.ShowLabels = value; ForceRedraw(); }
		}

		public event ControlAngleChangeCallback AngleChangeEvent;
		public event ControlScalingChangeCallback ScalingChangeEvent;

		private GraphicsElementGeometric3D lDOGraphics = new GraphicsElementGeometric3D();
		private int lCurrMouseX = 0;
		private int lCurrMouseY = 0;
		private bool lMouseDownDetect = false;
		private Rectangle lCtrlRect = new Rectangle(0, 0, 1, 1);

		public DesignObject3DControl()
		{
			InitializeComponent();

			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.ResizeRedraw, true);

			MouseWheel += new MouseEventHandler(lDesignObject3DControl_MouseWheelEvent);
		}

		public void ForceRedraw()
		{
			Invalidate();
		}

		public void ResetPosition()
		{
			lDOGraphics.Theta = GraphicsElementGeometric3D.ThetaDefault;
			lDOGraphics.Phi = GraphicsElementGeometric3D.PhiDefault;
			lDOGraphics.ScaleToElementSize(CreateGraphics());
			AngleChangeEvent(this, new ControlAngleChangeArgs(lDOGraphics.Phi, lDOGraphics.Theta));
			ScalingChangeEvent(this, new ControlScaleChangeArgs(lDOGraphics.Scaling));
			Invalidate();
		}

		public void SetDesignObject(DesignObject dobj)
		{
			lDOGraphics.Design = dobj;
			Invalidate();
		}

		private void DesignObject3DControl_Paint(object sender, PaintEventArgs e)
		{
			if (lDOGraphics != null)
			{
				lDOGraphics.PaintElement(e.Graphics, lCtrlRect, new Point(0, 0), false);
			}
		}

		private void DesignObject3DControl_Resize(object sender, EventArgs e)
		{
			lCtrlRect.Width = Width;
			lCtrlRect.Height = Height;
			lDOGraphics.Width = Width;
			lDOGraphics.Height = Height;
			lDOGraphics.ResetMidpoint();
		}

		private void DesignObject3DControl_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				lCurrMouseX = e.X;
				lCurrMouseY = e.Y;
				lMouseDownDetect = true;
			}
		}

		private void DesignObject3DControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (lMouseDownDetect)
			{
				lDOGraphics.Phi += (double)(e.X - lCurrMouseX) / 60.0F;
				lDOGraphics.Theta += (double)(e.Y - lCurrMouseY) / 60.0F;
				Refresh();
				lCurrMouseX = e.X;
				lCurrMouseY = e.Y;
				AngleChangeEvent(this, new ControlAngleChangeArgs(lDOGraphics.Phi, lDOGraphics.Theta));
			}
		}

		private void DesignObject3DControl_MouseUp(object sender, MouseEventArgs e)
		{
			lMouseDownDetect = false;
		}

		private void lDesignObject3DControl_MouseWheelEvent(object sender, MouseEventArgs e)
		{
			int scrllns = SystemInformation.MouseWheelScrollLines;
			double mvincr = ((double)e.Delta / 960.0F);
			double scalex = lDOGraphics.Scaling;
			int midx = lDOGraphics.MidpointX;
			int midy = lDOGraphics.MidpointY;

			if ((scalex + mvincr) > 0.01F)
			{
				double prex = (double)(e.X - midx) / scalex;
				double prey = (double)(e.Y - midy) / scalex;

				scalex += mvincr;

				int postx = (int)(prex * scalex) + midx;
				int posty = (int)(prey * scalex) + midy;
				int xdiff = (e.X - postx);
				int ydiff = (e.Y - posty);

				lDOGraphics.Scaling = scalex;
				lDOGraphics.MidpointX += xdiff;
				lDOGraphics.MidpointY += ydiff;

				ScalingChangeEvent(this, new ControlScaleChangeArgs(lDOGraphics.Scaling));

				Refresh();
			}
		}
	}
}
