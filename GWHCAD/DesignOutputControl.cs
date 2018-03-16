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
	public partial class DesignOutputControl : UserControl
	{
		public GraphicsDocument Document
		{
			get { return lDocument; }
		}

		private DesignObject lDesign = null;
		private GraphicsDocument lDocument = null;
		private Rectangle lCtrlRect = new Rectangle(0, 0, 1, 1);

		public DesignOutputControl()
		{
			InitializeComponent();

			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.ResizeRedraw, true);

			MouseWheel += new MouseEventHandler(lDesignOutputControl_MouseWheelEvent);
		}

		public void SetDesignObject(DesignObject design)
		{
			lDesign = design;
			UpdateDocument();
		}

		public void UpdateDocument()
		{
			lCreateDocumentFromDesign();
			Invalidate();
		}

		private void lCreateDocumentFromDesign()
		{
			if (lDesign != null)
			{
				if (lDocument == null)
				{
					lDocument = new GraphicsDocument();
				}
				else
				{
					lDocument.ClearDocument();
				}

				int curry = 0;
				Graphics gfx = CreateGraphics();

				lSetDocumentHeader(gfx, curry);
				// GWH UNFINISHED CODE--SHOW EACH ELEMENT INDIVIDUALLY
			}
		}

		private void lSetDocumentHeader(Graphics gfx, int curry)
		{
			if (lDocument != null)
			{
				lDocument.Elements.Add(new GraphicsElementText("Project:  " + lDesign.Name));
				lDocument.Elements.Add(new GraphicsElementText("Date:     " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString()));
				lDocument.Elements.Add(new GraphicsElementText(" "));
				lDocument.Elements.Add(new GraphicsElementText("3D View:"));
				lDocument.Elements.Add(new GraphicsElementGeometric3D(lDesign, gfx, Width, 300, GraphicsElementGeometric3D.PhiDefault, GraphicsElementGeometric3D.ThetaDefault, true, true));
				lDocument.Elements.Add(new GraphicsElementText(" "));

				GraphicsElementTable table = new GraphicsElementTable();
				table.AddRow(new GraphicsElementTableRow(new GraphicsElementTableCell[]
				{
					new GraphicsElementTableCell(new GraphicsElementText("Top View"), 0, 1, 1),
					new GraphicsElementTableCell(new GraphicsElementText("Front View"), 1, 1, 1),
					new GraphicsElementTableCell(new GraphicsElementText("Side View"), 2, 1, 1)
				}));
				table.AddRow(new GraphicsElementTableRow(new GraphicsElementTableCell[]
				{
					new GraphicsElementTableCell(new GraphicsElementGeometric3D(lDesign, gfx, Width, 200, (Math.PI / -2.0F), (3.0F * Math.PI) / 2.0F, true, false), 0, 1, 1),
					new GraphicsElementTableCell(new GraphicsElementGeometric3D(lDesign, gfx, Width, 200, (Math.PI / -2.0F), Math.PI, true, false), 1, 1, 1),
					new GraphicsElementTableCell(new GraphicsElementGeometric3D(lDesign, gfx, Width, 200, 0.0F, Math.PI, true, false), 2, 1, 1)
				}));
				lDocument.Elements.Add(table);
				lDocument.Elements.Add(new GraphicsElementText(" "));

				// add in design elements
				if (lDesign.Elements.Count > 0)
				{
					// list table of elements
					table = new GraphicsElementTable();
					table.AddRow(new GraphicsElementTableRow(new GraphicsElementTableCell[]
					{
						new GraphicsElementTableCell(new GraphicsElementText("Element"), 0, 1, 1),
						new GraphicsElementTableCell(new GraphicsElementText("Width (X)"), 1, 1, 1),
						new GraphicsElementTableCell(new GraphicsElementText("Depth (Y)"), 2, 1, 1),
						new GraphicsElementTableCell(new GraphicsElementText("Height (Z)"), 3, 1, 1)
					}));
					foreach (DesignElement tmpde in lDesign.Elements)
					{
						table.AddRow(new GraphicsElementTableRow(new GraphicsElementTableCell[]
						{
							new GraphicsElementTableCell(new GraphicsElementText(tmpde.Name), 0, 1, 1),
							new GraphicsElementTableCell(new GraphicsElementText(tmpde.DimensionX.ToString()), 1, 1, 1),
							new GraphicsElementTableCell(new GraphicsElementText(tmpde.DimensionY.ToString()), 2, 1, 1),
							new GraphicsElementTableCell(new GraphicsElementText(tmpde.DimensionZ.ToString()), 3, 1, 1)
						}));
					}
					lDocument.Elements.Add(table);

					// add specific drawings of each element
					foreach (DesignElement tmpde in lDesign.Elements)
					{
						lDocument.Elements.Add(new GraphicsElementText(" "));
						table = new GraphicsElementTable();
						table.AddRow(new GraphicsElementTableRow(new GraphicsElementTableCell[]
						{
							new GraphicsElementTableCell(new GraphicsElementText(tmpde.Name + ":"), 0, 4, 1)
						}));
						table.AddRow(new GraphicsElementTableRow(new GraphicsElementTableCell[]
						{
							new GraphicsElementTableCell(new GraphicsElementText("Parameters"), 0, 1, 1),
							new GraphicsElementTableCell(new GraphicsElementText("Top View"), 1, 1, 1),
							new GraphicsElementTableCell(new GraphicsElementText("Front View"), 2, 1, 1),
							new GraphicsElementTableCell(new GraphicsElementText("Side View"), 3, 1, 1),
						}));
						table.AddRow(new GraphicsElementTableRow(new GraphicsElementTableCell[]
						{
							new GraphicsElementTableCell(new GraphicsElementText("Width (X):  " + tmpde.DimensionX), 0, 1, 1),
							new GraphicsElementTableCell(new GraphicsElementGeometric3D(lDesign, tmpde.Name, gfx, 200, 200, (Math.PI / -2.0F), (3.0F * Math.PI) / 2.0F, true, false), 1, 1, 3),
							new GraphicsElementTableCell(new GraphicsElementGeometric3D(lDesign, tmpde.Name, gfx, 200, 200, (Math.PI / -2.0F), Math.PI, true, false), 2, 1, 3),
							new GraphicsElementTableCell(new GraphicsElementGeometric3D(lDesign, tmpde.Name, gfx, 200, 200, 0.0F, Math.PI, true, false), 3, 1, 3),
						}));
						table.AddRow(new GraphicsElementTableRow(new GraphicsElementTableCell[]
						{
							new GraphicsElementTableCell(new GraphicsElementText("Depth (Y):  " + tmpde.DimensionY), 0, 1, 1)
						}));
						table.AddRow(new GraphicsElementTableRow(new GraphicsElementTableCell[]
						{
							new GraphicsElementTableCell(new GraphicsElementText("Height (Z): " + tmpde.DimensionZ), 0, 1, 1)
						}));
						lDocument.Elements.Add(table);
					}
				}

				lDocument.SetDocumentDimensions(gfx);
			}
		}

		private void lDesignOutputControl_MouseWheelEvent(object sender, MouseEventArgs e)
		{

		}

		private void DesignOutputControl_Resize(object sender, EventArgs e)
		{
			lCtrlRect.Width = Width;
			lCtrlRect.Height = Height;

			lVScrollBar.LargeChange = Height - lHScrollBar.Height;
			lHScrollBar.LargeChange = Width - lVScrollBar.Width;

			// update the scroll bars
			lVScrollBar.Maximum = (lDocument == null ? 0 : lDocument.Height);
			lVScrollBar.LargeChange = Height - lHScrollBar.Height;
			lVScrollBar.Value = lCtrlRect.Y;

			lHScrollBar.Maximum = (lDocument == null ? 0 : lDocument.Width);
			lHScrollBar.LargeChange = Width - lVScrollBar.Width;
			lHScrollBar.Value = lCtrlRect.X;

			//lDocument.SetDocumentDimensions(CreateGraphics());
		}

		private void DesignOutputControl_Paint(object sender, PaintEventArgs e)
		{
			if (lDocument != null)
			{
				lDocument.PaintElement(e.Graphics, lCtrlRect, new Point(0, 0), false);
			}
		}

		private void lVScrollBar_Scroll(object sender, ScrollEventArgs e)
		{
			lCtrlRect.Y += (e.NewValue - e.OldValue);
			Invalidate();
		}

		private void lHScrollBar_Scroll(object sender, ScrollEventArgs e)
		{
			lCtrlRect.X += (e.NewValue - e.OldValue);
			Invalidate();
		}
	}
}
