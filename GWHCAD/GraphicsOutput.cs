/************************************************************************************

	Project:
		GWHCAD

	File:
		GraphicsOutput.cs

	Description:
		This souce file contains the graphics class for displaying graphics items,
		including drawing elements in the output form.

	MIT License

	Copyright (c) 2018 Greg Hall

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
	SOFTWARE.

************************************************************************************/


// Namespaces used in this source
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GWHCAD
{
	public class GraphicsElement
	{
		public enum ElementTypes
		{
			GenericElement,
			Text,
			Geometric3D,
			Table,
			TableRow,
			TableCell,
			Document,
			Layout
		}

		public int OffsetX = 0;				// the offset of the origin of the element from origin of the drawing area
		public int OffsetY = 0;             // the offset of the origin of the element from origin of the drawing area
		public int Width = 0;
		public int Height = 0;
		public int EffectiveWidth = 0;
		public int EffectiveHeight = 0;
		public ElementTypes ElementType = ElementTypes.GenericElement;

		public GraphicsElement()
		{
			// do nothing for now
		}

		public virtual int PaintElement(Graphics gfx, Rectangle displayrect, Point origintranslate, bool printing) { return -1; }
		public virtual void RecalcSize(Graphics gfx) { }
		public virtual void ScaleToElementSize(Graphics gfx) { }
		public virtual void UpdateLocation(int offsetx, int offsety)
		{
			OffsetX = offsetx;
			OffsetY = offsety;
		}
	}

	public class GraphicsElementText : GraphicsElement
	{
		public string Text = "";
		public Font TextFont = new Font("Segoe UI", 10.0F);
		public Brush TextColor = new SolidBrush(Color.Black);
		public Brush BackColor = null;                          // null will be interpreted as "transparent background"

		private StringFormat lStringFormat = null;

		public GraphicsElementText()
		{
			lInitializeVariables();
		}

		public GraphicsElementText(string text)
		{
			lInitializeVariables();
			Text = text;
		}

		private void lInitializeVariables()
		{
			ElementType = ElementTypes.Text;
			lStringFormat = new StringFormat(StringFormat.GenericTypographic);
			lStringFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
		}

		public override int PaintElement(Graphics gfx, Rectangle displayrect, Point origintranslate, bool printing)
		{
			int outy = OffsetY - displayrect.Y + origintranslate.Y;
			int outx = OffsetX - displayrect.X + origintranslate.X;

			if (BackColor != null)
			{
				gfx.FillRectangle(BackColor, outx, outy, Width, Height);
			}

			gfx.DrawString(Text, TextFont, TextColor, (float)outx, (float)outy, lStringFormat);

			return OffsetY + Height;
		}

		public override void RecalcSize(Graphics gfx)
		{
			SizeF strsz = gfx.MeasureString(Text, TextFont);
			Width = (int)strsz.Width;
			Height = (int)strsz.Height;
			EffectiveWidth = Width;
			EffectiveHeight = Height;
		}
	}

	public class GraphicsElementGeometric3D : GraphicsElement
	{
		public DesignObject Design = null;
		public Font LabelFont = new Font("Segoe UI", 10.0F);

		public double Theta
		{
			get { return lTheta; }
			set { lTheta = value; }
		}
		public double Phi
		{
			get { return lPhi; }
			set { lPhi = value; }
		}
		public static double ThetaDefault
		{
			get { return lThetaDefault; }
		}
		public static double PhiDefault
		{
			get { return lPhiDefault; }
		}
		public double Scaling
		{
			get { return lScaling; }
			set { lScaling = value; }
		}
		public int MidpointX
		{
			get { return lMidpointX; }
			set { lMidpointX = value; }
		}
		public int MidpointY
		{
			get { return lMidpointY; }
			set { lMidpointY = value; }
		}
		public bool ShowAxes
		{
			get { return lShowAxes; }
			set { lShowAxes = value; }
		}
		public bool ShowLabels
		{
			get { return lShowLabels; }
			set { lShowLabels = value; }
		}

		private static double lThetaDefault = (8 * Math.PI) / 7;//180.0F;
		private static double lPhiDefault = (Math.PI / -4.0F);//90.0F;
		private double lTheta = (8 * Math.PI) / 7;//180.0F;
		private double lPhi = (Math.PI / -4.0F);//90.0F;
		private int lMidpointX = -1;
		private int lMidpointY = -1;
		private double lScaling = 4.0F;

		private Font lLabelFont = new Font("Segoe UI", 10.0F);
		private SolidBrush lFillbrush = new SolidBrush(Color.FromArgb((int)0x80, Color.Brown));
		private Pen lOutlinepen = new Pen(Color.Black);

		private bool lShowAxes = false;
		private bool lShowLabels = false;

		private Point3D[] lAxisX = new Point3D[2];
		private Point3D[] lAxisY = new Point3D[2];
		private Point3D[] lAxisZ = new Point3D[2];
		private Point[] lTempPanelPoints = new Point[4];

		private string lSingleElement = "";


		public GraphicsElementGeometric3D()
		{
			lInitializeVariables();
		}

		public GraphicsElementGeometric3D(DesignObject design)
		{
			lInitializeVariables();
			Design = design;
		}

		public GraphicsElementGeometric3D(DesignObject design, int width, int height, double phi, double theta, bool showaxes)
		{
			lInitializeVariables();
			Design = design;
			Width = width;
			Height = height;
			lPhi = phi;
			lTheta = theta;
		}

		public GraphicsElementGeometric3D(DesignObject design, int width, int height, int offsetx, int offsety, double phi, double theta)
		{
			lInitializeVariables();
			Design = design;
			Width = width;
			Height = height;
			OffsetX = offsetx;
			OffsetY = offsety;
			lPhi = phi;
			lTheta = theta;
		}

		public GraphicsElementGeometric3D(DesignObject design, Graphics gfx, int width, int height, double phi, double theta, bool showaxes, bool showlabels)
		{
			lInitializeVariables();
			Design = design;
			Width = width;
			Height = height;
			lPhi = phi;
			lTheta = theta;
			lShowAxes = showaxes;
			lShowLabels = showlabels;
			ScaleToElementSize(gfx);
		}

		public GraphicsElementGeometric3D(DesignObject design, string element, Graphics gfx, int width, int height, double phi, double theta, bool showaxes, bool showlabels)
		{
			lInitializeVariables();
			Design = design;
			lSingleElement = element;
			Width = width;
			Height = height;
			lPhi = phi;
			lTheta = theta;
			lShowAxes = showaxes;
			lShowLabels = showlabels;
			ScaleToElementSize(gfx);
		}

		public GraphicsElementGeometric3D(DesignObject design, Graphics gfx, int width, int height, int offsetx, int offsety, double phi, double theta, bool showaxes, bool showlabels)
		{
			lInitializeVariables();
			Design = design;
			Width = width;
			Height = height;
			OffsetX = offsetx;
			OffsetY = offsety;
			lPhi = phi;
			lTheta = theta;
			lShowAxes = showaxes;
			lShowLabels = showlabels;
			ScaleToElementSize(gfx);
		}

		public GraphicsElementGeometric3D(DesignObject design, string element, Graphics gfx, int width, int height, int offsetx, int offsety, double phi, double theta, bool showaxes, bool showlabels)
		{
			lInitializeVariables();
			Design = design;
			lSingleElement = element;
			Width = width;
			Height = height;
			OffsetX = offsetx;
			OffsetY = offsety;
			lPhi = phi;
			lTheta = theta;
			lShowAxes = showaxes;
			lShowLabels = showlabels;
			ScaleToElementSize(gfx);
		}

		public override int PaintElement(Graphics gfx, Rectangle displayrect, Point origintranslate, bool printing)
		{
			int outy = OffsetY - displayrect.Y + origintranslate.Y;
			int outx = OffsetX - displayrect.X + origintranslate.X;
			int tofstx = lMidpointX + OffsetX - displayrect.X + origintranslate.X;
			int tofsty = lMidpointY + OffsetY - displayrect.Y + origintranslate.Y;

			// draw all elements
			if ((Design != null) && (Design.Elements.Count > 0))
			{
				foreach (DesignElement tmpde in Design.Elements)
				{
					if ((lSingleElement.Length == 0) || (lSingleElement.Equals(tmpde.Name)))
					{
						if (tmpde.ElementType == DesignElement.ElementTypes.Rect)
						{
							lDrawDesignElement(gfx, tmpde, tofstx, tofsty);

							// draw label
							if (lShowLabels)
							{
								Point3D lblpt = Graphics3D.TransformSinglePoint(Phi, Theta, tmpde.Midpoint, 0.0F);
								gfx.DrawString(tmpde.Name, LabelFont, Brushes.Black, new PointF((float)(lblpt.X * lScaling) + tofstx, (float)(lblpt.Y * lScaling) + tofsty));
							}
						}
					}
				}
			}

			// draw coordinates
			if (lShowAxes)
			{
				Pen outlinepen = new Pen(Color.Black);

				SetAxisPoints();

				// draw X cooridate
				Point3D pt1 = Graphics3D.TransformSinglePoint(Phi, Theta, lAxisX[0], 0.0F);
				Point3D pt2 = Graphics3D.TransformSinglePoint(Phi, Theta, lAxisX[1], 0.0F);

				PointF gpt1 = new PointF((float)(pt1.X * lScaling) + tofstx, (float)(pt1.Y * lScaling) + tofsty);
				PointF gpt2 = new PointF((float)(pt2.X * lScaling) + tofstx, (float)(pt2.Y * lScaling) + tofsty);

				gfx.DrawLine(outlinepen, gpt1, gpt2);// (float)(pt1.X * lScaling) + lMidpointX, (float)(pt1.Y * lScaling) + lMidpointX, (float)(pt2.X * lScaling) +lMidpointX, (float)(pt2.Y * lScaling) +lMidpointX);

				gfx.DrawString("+X", LabelFont, Brushes.Black, gpt1);
				gfx.DrawString("-X", LabelFont, Brushes.Black, gpt2);

				// draw Y cooridate
				pt1 = Graphics3D.TransformSinglePoint(Phi, Theta, lAxisY[0], 0.0F);
				pt2 = Graphics3D.TransformSinglePoint(Phi, Theta, lAxisY[1], 0.0F);

				gpt1.X = (float)(pt1.X * lScaling) + tofstx;
				gpt1.Y = (float)(pt1.Y * lScaling) + tofsty;
				gpt2.X = (float)(pt2.X * lScaling) + tofstx;
				gpt2.Y = (float)(pt2.Y * lScaling) + tofsty;

				gfx.DrawLine(outlinepen, gpt1, gpt2);// (float)(pt1.X * lScaling) + lMidpointX, (float)(pt1.Y * lScaling) + lMidpointX, (float)(pt2.X * lScaling) +lMidpointX, (float)(pt2.Y * lScaling) +lMidpointX);

				gfx.DrawString("+Y", LabelFont, Brushes.Black, gpt1);
				gfx.DrawString("-Y", LabelFont, Brushes.Black, gpt2);

				// draw Z cooridate
				pt1 = Graphics3D.TransformSinglePoint(Phi, Theta, lAxisZ[0], 0.0F);
				pt2 = Graphics3D.TransformSinglePoint(Phi, Theta, lAxisZ[1], 0.0F);

				gpt1.X = (float)(pt1.X * lScaling) + tofstx;
				gpt1.Y = (float)(pt1.Y * lScaling) + tofsty;
				gpt2.X = (float)(pt2.X * lScaling) + tofstx;
				gpt2.Y = (float)(pt2.Y * lScaling) + tofsty;

				gfx.DrawLine(outlinepen, gpt1, gpt2);// (float)(pt1.X * lScaling) + lMidpointX, (float)(pt1.Y * lScaling) + lMidpointX, (float)(pt2.X * lScaling) +lMidpointX, (float)(pt2.Y * lScaling) +lMidpointX);

				gfx.DrawString("+Z", LabelFont, Brushes.Black, gpt1);
				gfx.DrawString("-Z", LabelFont, Brushes.Black, gpt2);
			}

			return OffsetY + Height;
		}

		public override void RecalcSize(Graphics gfx)
		{
			ScaleToElementSize(gfx);
		}

		public void ResetMidpoint()
		{
			MidpointX = Width / 2;
			MidpointY = Height / 2;
		}

		public override void ScaleToElementSize(Graphics gfx)
		{
			double minx = 0;
			double maxx = 0;
			double miny = 0;
			double maxy = 0;
			bool valset = false;

			ResetMidpoint();

			// measure all elements
			if ((Design != null) && (Design.Elements.Count > 0))
			{
				foreach (DesignElement tmpde in Design.Elements)
				{
					if (tmpde.ElementType == DesignElement.ElementTypes.Rect)
					{
						int tmpndx;

						Graphics3D.TransformDesignElement(lPhi, lTheta, tmpde, 0.0F);

						if (!valset)
						{
							minx = tmpde.ProjectedPlanes[0].Points[0].X;
							maxx = tmpde.ProjectedPlanes[0].Points[0].X;
							miny = tmpde.ProjectedPlanes[0].Points[0].Y;
							maxy = tmpde.ProjectedPlanes[0].Points[0].Y;
							valset = true;
						}

						foreach (RectPlane tmpplane in tmpde.ProjectedPlanes)
						{
							for (tmpndx = 0; tmpndx < tmpplane.Points.Length; tmpndx++)
							{
								if (minx > tmpplane.Points[tmpndx].X) minx = tmpplane.Points[tmpndx].X;
								if (maxx < tmpplane.Points[tmpndx].X) maxx = tmpplane.Points[tmpndx].X;
								if (miny > tmpplane.Points[tmpndx].Y) miny = tmpplane.Points[tmpndx].Y;
								if (maxy < tmpplane.Points[tmpndx].Y) maxy = tmpplane.Points[tmpndx].Y;
							}
						}

						// measure label
						//Point3D lblpt = Graphics3D.TransformSinglePoint(lPhi, lTheta, tmpde.Midpoint, 0.0F);
						//SizeF strsz = gfx.MeasureString(tmpde.Name, LabelFont);
						//if (minx > lblpt.X)
						//	minx = lblpt.X;
						//if (maxx < lblpt.X)
						//	maxx = lblpt.X;
						//if (miny > lblpt.Y)
						//	miny = lblpt.Y;
						//if (maxy < lblpt.Y)
						//	maxy = lblpt.Y;
					}
				}
			}

			// draw coordinates
			if (ShowAxes)
			{
				SetAxisPoints();

				// draw X cooridate
				Point3D pt1 = Graphics3D.TransformSinglePoint(Phi, Theta, lAxisX[0], 0.0F);
				Point3D pt2 = Graphics3D.TransformSinglePoint(Phi, Theta, lAxisX[1], 0.0F);

				if (minx > pt1.X) minx = pt1.X;
				if (maxx < pt1.X) maxx = pt1.X;
				if (miny > pt1.Y) miny = pt1.Y;
				if (maxy < pt1.Y) maxy = pt1.Y;

				if (minx > pt2.X) minx = pt2.X;
				if (maxx < pt2.X) maxx = pt2.X;
				if (miny > pt2.Y) miny = pt2.Y;
				if (maxy < pt2.Y) maxy = pt2.Y;

				//gfx.DrawString("+X", LabelFont, Brushes.Black, gpt1);
				//gfx.DrawString("-X", LabelFont, Brushes.Black, gpt2);

				// draw Y cooridate
				pt1 = Graphics3D.TransformSinglePoint(Phi, Theta, lAxisY[0], 0.0F);
				pt2 = Graphics3D.TransformSinglePoint(Phi, Theta, lAxisY[1], 0.0F);

				if (minx > pt1.X) minx = pt1.X;
				if (maxx < pt1.X) maxx = pt1.X;
				if (miny > pt1.Y) miny = pt1.Y;
				if (maxy < pt1.Y) maxy = pt1.Y;

				if (minx > pt2.X) minx = pt2.X;
				if (maxx < pt2.X) maxx = pt2.X;
				if (miny > pt2.Y) miny = pt2.Y;
				if (maxy < pt2.Y) maxy = pt2.Y;

				//gfx.DrawString("+Y", LabelFont, Brushes.Black, gpt1);
				//gfx.DrawString("-Y", LabelFont, Brushes.Black, gpt2);

				// draw Z cooridate
				pt1 = Graphics3D.TransformSinglePoint(Phi, Theta, lAxisZ[0], 0.0F);
				pt2 = Graphics3D.TransformSinglePoint(Phi, Theta, lAxisZ[1], 0.0F);

				if (minx > pt1.X) minx = pt1.X;
				if (maxx < pt1.X) maxx = pt1.X;
				if (miny > pt1.Y) miny = pt1.Y;
				if (maxy < pt1.Y) maxy = pt1.Y;

				if (minx > pt2.X) minx = pt2.X;
				if (maxx < pt2.X) maxx = pt2.X;
				if (miny > pt2.Y) miny = pt2.Y;
				if (maxy < pt2.Y) maxy = pt2.Y;

				//gfx.DrawString("+Z", LabelFont, Brushes.Black, gpt1);
				//gfx.DrawString("-Z", LabelFont, Brushes.Black, gpt2);
			}

			double xdiff = maxx - minx;
			double ydiff = maxy - miny;
			double scalex = (double)Width / xdiff;
			double scaley = (double)Height / ydiff;

			lScaling = (scalex > scaley ? scaley : scalex);
			EffectiveWidth = (int)(xdiff * lScaling);
			EffectiveHeight = (int)(ydiff * lScaling);
		}

		public void SetAxisPoints()
		{
			if (Design != null)
			{
				Point3D maxpts = Design.GetMaxCoordinates();
				Point3D minpts = Design.GetMinCoordinates();

				lAxisX[0].SetPoint(maxpts.X * 1.25F, 0.0F, 0.0F);
				lAxisX[1].SetPoint(minpts.X * 1.25F, 0.0F, 0.0F);
				lAxisY[0].SetPoint(0.0F, maxpts.Y * 1.25F, 0.0F);
				lAxisY[1].SetPoint(0.0F, minpts.Y * 1.25F, 0.0F);
				lAxisZ[0].SetPoint(0.0F, 0.0F, maxpts.Z * 1.25F);
				lAxisZ[1].SetPoint(0.0F, 0.0F, minpts.Z * 1.25F);
			}
		}

		public void ValidateMidpoint()
		{
			if (lMidpointX < 0)
			{
				lMidpointX = Width / 2;
			}
			if (lMidpointY < 0)
			{
				lMidpointY = Height / 2;
			}
		}

		private void lDrawDesignElement(Graphics gfx, DesignElement delement, int offsetx, int offsety)
		{
			int tmpndx;

			Graphics3D.TransformDesignElement(lPhi, lTheta, delement, 0.0F);

			foreach (RectPlane tmpplane in delement.ProjectedPlanes)
			{
				for (tmpndx = 0; tmpndx < tmpplane.Points.Length; tmpndx++)
				{
					lTempPanelPoints[tmpndx].X = (int)(tmpplane.Points[tmpndx].X * lScaling) + offsetx;
					lTempPanelPoints[tmpndx].Y = (int)(tmpplane.Points[tmpndx].Y * lScaling) + offsety;
				}

				gfx.FillPolygon(lFillbrush, lTempPanelPoints);
				gfx.DrawLine(lOutlinepen, lTempPanelPoints[0], lTempPanelPoints[1]);
				gfx.DrawLine(lOutlinepen, lTempPanelPoints[1], lTempPanelPoints[2]);
				gfx.DrawLine(lOutlinepen, lTempPanelPoints[2], lTempPanelPoints[3]);
				gfx.DrawLine(lOutlinepen, lTempPanelPoints[3], lTempPanelPoints[0]);
			}
		}

		private void lInitializeVariables()
		{
			ElementType = ElementTypes.Geometric3D;

			lTheta = lThetaDefault;
			lPhi = lPhiDefault;

			lAxisX[0] = new Point3D();
			lAxisX[1] = new Point3D();
			lAxisY[0] = new Point3D();
			lAxisY[1] = new Point3D();
			lAxisZ[0] = new Point3D();
			lAxisZ[1] = new Point3D();

			int tmpndx;

			for (tmpndx = 0; tmpndx < lTempPanelPoints.Length; tmpndx++)
			{
				lTempPanelPoints[tmpndx] = new Point(0, 0);
			}
		}
	}

	public class GraphicsElementCellDiffs
	{
		public int Left = 0;
		public int Top = 0;
		public int Right = 0;
		public int Bottom = 0;

		public GraphicsElementCellDiffs()
		{
			// do nothing
		}

		public GraphicsElementCellDiffs(int left, int right, int top, int bottom)
		{
			Left = left;
			Right = right;
			Top = top;
			Bottom = bottom;
		}
	}

	public enum GraphicsElementVAlign
	{
		Top,
		Middle,
		Bottom,
		Full
	}

	public enum GraphicsElementHAlign
	{
		Left,
		Center,
		Right,
		Full
	}

	public class GraphicsElementTableCell : GraphicsElement
	{
		public GraphicsElement CellElement = null;
		public int Column = 0;
		public int ColSpan = 1;
		public int RowSpan = 1;
		public GraphicsElementCellDiffs Padding = new GraphicsElementCellDiffs(5, 5, 5, 5);
		public GraphicsElementCellDiffs Margins = new GraphicsElementCellDiffs(0, 0, 0, 0);
		public GraphicsElementVAlign VAlign = GraphicsElementVAlign.Top;
		public GraphicsElementHAlign HAlign = GraphicsElementHAlign.Left;

		public GraphicsElementTableCell()
		{
			ElementType = ElementTypes.TableCell;
		}

		public GraphicsElementTableCell(GraphicsElement element, int column, int colspan, int rowspan)
		{
			CellElement = element;
			Column = column;
			ColSpan = colspan;
			RowSpan = rowspan;
		}

		public override int PaintElement(Graphics gfx, Rectangle displayrect, Point origintranslate, bool printing)
		{
			int outy = 0;

			if (CellElement != null)
			{
				outy = CellElement.PaintElement(gfx, displayrect, origintranslate, printing);
			}

			return outy;
		}

		public override void RecalcSize(Graphics gfx)
		{
			Width = 0;
			Height = 0;
			EffectiveWidth = 0;
			EffectiveHeight = 0;

			if (CellElement != null)
			{
				CellElement.RecalcSize(gfx);
				CellElement.Width = CellElement.EffectiveWidth;
				CellElement.Height = CellElement.EffectiveHeight;

				if (CellElement.ElementType == ElementTypes.Geometric3D)
				{
					GraphicsElementGeometric3D obj3d = (GraphicsElementGeometric3D)CellElement;
					obj3d.ResetMidpoint();
				}

				if (Width == 0)
					Width = CellElement.Width;
				if (Height == 0)
					Height = CellElement.Height;
			}

			Width += Padding.Left + Padding.Right;
			Height += Padding.Top + Padding.Bottom;
			EffectiveWidth = Width;
			EffectiveHeight = Height;
		}

		public override void UpdateLocation(int offsetx, int offsety)
		{
			OffsetX = offsetx;
			OffsetY = offsety;
			CellElement.UpdateLocation(offsetx + Padding.Left, offsety + Padding.Top);
		}
	}

	public class GraphicsElementTableRow : GraphicsElement
	{
		public List<GraphicsElementTableCell> Cells = new List<GraphicsElementTableCell>();
		public int[] ColumnOffsets = null;

		public GraphicsElementTableRow()
		{
			ElementType = ElementTypes.TableRow;
		}

		public GraphicsElementTableRow(GraphicsElementTableCell[] cells)
		{
			ElementType = ElementTypes.TableRow;

			if ((cells != null) && (cells.Length > 0))
			{
				foreach (GraphicsElementTableCell tmpcell in cells)
				{
					Cells.Add(tmpcell);
				}
			}
		}

		public int GetMaxColumn()
		{
			int maxcol = 0;

			if (Cells.Count > 0)
			{
				foreach (GraphicsElementTableCell tmpcell in Cells)
				{
					int tmpmaxcol = tmpcell.Column + tmpcell.ColSpan;

					if (maxcol < tmpmaxcol)
						maxcol = tmpmaxcol;
				}
			}

			return maxcol;
		}

		public override int PaintElement(Graphics gfx, Rectangle displayrect, Point origintranslate, bool printing)
		{
			if (Cells.Count > 0)
			{
				foreach (GraphicsElementTableCell tmpcell in Cells)
				{
					tmpcell.PaintElement(gfx, displayrect, origintranslate, printing);
				}
			}

			return OffsetY + Height;
		}

		public override void RecalcSize(Graphics gfx)
		{
			Height = 0;
			Width = 0;
			EffectiveHeight = 0;
			EffectiveWidth = 0;

			if (Cells.Count > 0)
			{
				// reset each cell's size and attempt to get the max height and determine the minimum width needed
				foreach (GraphicsElementTableCell tmpcell in Cells)
				{
					tmpcell.RecalcSize(gfx);

					if ((tmpcell.RowSpan == 1) && (EffectiveHeight < tmpcell.Height))
						EffectiveHeight = tmpcell.Height;
				}

				// reset each cell's height to match the row height
				foreach (GraphicsElementTableCell tmpcell in Cells)
				{
					if ((tmpcell.RowSpan == 1) && (Height < tmpcell.Height))
						tmpcell.Height = EffectiveHeight;
				}

				Height = EffectiveHeight;
			}
		}

		public void SetColumnOffsets(int[] coloffsets)
		{
			ColumnOffsets = coloffsets;
		}

		public override void UpdateLocation(int offsetx, int offsety)
		{
			OffsetY = offsety;
			OffsetX = offsetx;

			if ((ColumnOffsets != null) && (Cells.Count > 0))
			{
				foreach (GraphicsElementTableCell tmpcell in Cells)
				{
					tmpcell.UpdateLocation(ColumnOffsets[tmpcell.Column], offsety);
				}
			}
		}
	}

	public class GraphicsElementTable : GraphicsElement
	{
		public List<GraphicsElementTableRow> Rows = new List<GraphicsElementTableRow>();
		public int[] ColumnWidths = new int[0];

		public int RowCount
		{
			get { return lRowCount; }
		}
		public int ColumnCount
		{
			get { return lColumnCount; }
		}

		private int lRowCount = 0;
		private int lColumnCount = 0;

		public GraphicsElementTable()
		{
			ElementType = ElementTypes.Table;
		}

		public void AddRow(GraphicsElementTableRow row)
		{
			row.OffsetX = OffsetX;
			Rows.Add(row);
			lRowCount = Rows.Count;

			int rowcolcnt = row.GetMaxColumn();

			if (lColumnCount < rowcolcnt)
				lColumnCount = rowcolcnt;
		}

		public override int PaintElement(Graphics gfx, Rectangle displayrect, Point origintranslate, bool printing)
		{
			if (Rows.Count > 0)
			{
				foreach (GraphicsElementTableRow tmprow in Rows)
				{
					tmprow.PaintElement(gfx, displayrect, origintranslate, printing);
				}
			}

			return OffsetY + Height;
		}

		public override void RecalcSize(Graphics gfx)
		{
			if (Rows.Count > 0)
			{
				// recalculate base cell sizes, and get the number of columns
				foreach (GraphicsElementTableRow tmprow in Rows)
				{
					tmprow.Height = 0;

					if (tmprow.Cells.Count > 0)
					{
						foreach (GraphicsElementTableCell tmpcell in tmprow.Cells)
						{
							tmpcell.RecalcSize(gfx);

							if ((tmpcell.RowSpan == 1) && (tmprow.Height < tmpcell.Height))
								tmprow.Height = tmpcell.Height;
						}
					}
				}

				// set the column widths and row heights
				lClearColumnWidths();
				lUpdateColumnWidthsAndRowHeights();
			}
		}


		public override void UpdateLocation(int offsetx, int offsety)
		{
			OffsetX = offsetx;
			OffsetY = offsety;

			if (Rows.Count > 0)
			{
				int[] coloffsets = new int[ColumnWidths.Length];

				// set column offsets
				int colndx = 0;
				int cofstx = OffsetX;
				Width = 0;

				while (colndx < ColumnWidths.Length)
				{
					coloffsets[colndx] = cofstx;
					cofstx += ColumnWidths[colndx];
					colndx++;
				}

				int offsty = OffsetY;

				// update cell offsets
				foreach (GraphicsElementTableRow tmprow in Rows)
				{
					tmprow.SetColumnOffsets(coloffsets);
					tmprow.UpdateLocation(OffsetX, offsty);

					offsty += tmprow.Height;
				}
			}
		}

		private void lClearColumnWidths()
		{
			if (ColumnWidths.Length != lColumnCount)
			{
				ColumnWidths = new int[lColumnCount];
			}

			int colndx = lColumnCount;

			while (colndx > 0)
			{
				colndx--;
				ColumnWidths[colndx] = 0;
			}
		}

		private void lUpdateColumnWidthsAndRowHeights()
		{
			if (Rows.Count > 0)
			{
				int[] coloffsets = new int[lColumnCount];

				// set the base column widths
				foreach (GraphicsElementTableRow tmprow in Rows)
				{
					if (tmprow.Cells.Count > 0)
					{
						foreach (GraphicsElementTableCell tmpcell in tmprow.Cells)
						{
							if ((tmpcell.ColSpan == 1) && (ColumnWidths[tmpcell.Column] < tmpcell.Width))
							{
								ColumnWidths[tmpcell.Column] = tmpcell.Width;
							}
						}
					}
				}

				// adjust the column widths for cells that span multiple columns
				foreach (GraphicsElementTableRow tmprow in Rows)
				{
					if (tmprow.Cells.Count > 0)
					{
						foreach (GraphicsElementTableCell tmpcell in tmprow.Cells)
						{
							if (tmpcell.ColSpan > 1)
							{
								int tmpcnt = 0;
								int tmpwidth = 0;

								do
								{
									tmpwidth += ColumnWidths[tmpcell.Column + tmpcnt];
									tmpcnt++;
								} while (tmpcnt < tmpcell.ColSpan);

								if (tmpwidth < tmpcell.Width)
								{
									ColumnWidths[tmpcell.Column + (tmpcell.ColSpan - 1)] = tmpcell.Width - tmpwidth;
									// GWH FUTURE ENHANCEMENT--SPREAD THE EXTRA WIDTH OVER MULTIPLE COLUMNS TO MAKE THE COLUMNS APPEAR MORE EVEN
								}
								else
								{
									tmpcell.Width = tmpwidth;
								}
							}
						}
					}
				}

				// set column offsets
				int colndx = 0;
				Width = 0;

				while (colndx < ColumnWidths.Length)
				{
					coloffsets[colndx] = Width;
					Width += ColumnWidths[colndx];
					colndx++;
				}

				// update all cell's column widths and offsetx positions
				int offsty = 0;

				foreach (GraphicsElementTableRow tmprow in Rows)
				{
					if (tmprow.Cells.Count > 0)
					{
						foreach (GraphicsElementTableCell tmpcell in tmprow.Cells)
						{
							if (tmpcell.ColSpan == 1)
							{
								tmpcell.Width = ColumnWidths[tmpcell.Column];
							}

							if (tmpcell.RowSpan == 1)
							{
								tmpcell.Height = tmprow.Height;
							}
						}
					}

					offsty += tmprow.Height;
				}

				// update multi-row cell heights
				int rowndx = 0;
				while (rowndx < Rows.Count)
				{
					GraphicsElementTableRow tmprow = Rows[rowndx];

					if (tmprow.Cells.Count > 0)
					{
						int baserowht = tmprow.Height;

						foreach (GraphicsElementTableCell tmpcell in tmprow.Cells)
						{
							if (tmpcell.RowSpan > 1)
							{
								int tmprowndx = 0;
								int tmprowht = 0;

								while (tmprowndx < tmpcell.RowSpan)
								{
									tmprowht += Rows[rowndx + tmprowndx].Height;
									tmprowndx++;
								}

								if (tmpcell.Height > tmprowht)
								{
									Rows[rowndx + (tmpcell.RowSpan - 1)].Height += tmpcell.Height - tmprowht;
								}
							}
							else if (tmpcell.Height < baserowht)
							{
								tmpcell.Height = baserowht;
							}
						}
					}

					rowndx++;
				}

				// update cell offsets and the total table height
				Height = 0;
				foreach (GraphicsElementTableRow tmprow in Rows)
				{
					if (tmprow.Cells.Count > 0)
					{
						foreach (GraphicsElementTableCell tmpcell in tmprow.Cells)
						{
							tmpcell.OffsetX = coloffsets[tmpcell.Column];
							tmpcell.OffsetY = Height;
						}
					}

					tmprow.OffsetY = Height;
					Height += tmprow.Height;
				}
			}
		}
	}

	public class GraphicsDocument : GraphicsElement
	{
		public List<GraphicsElement> Elements = new List<GraphicsElement>();

		public GraphicsDocument()
		{
			ElementType = ElementTypes.Document;
		}

		public void ClearDocument()
		{
			Elements.Clear();
			Width = 0;
			Height = 0;
		}

		public override int PaintElement(Graphics gfx, Rectangle displayrect, Point origintranslate, bool printing)
		{
			int dsply = displayrect.Y;
			int dsplh = displayrect.Height;
			int dsplymax = dsply + dsplh;
			int endy = 0;

			//SetElementOffsets(gfx);
			SetElementOffsets(gfx);

			// paint each element that will actually show up in the display rectangle
			foreach (GraphicsElement tmpelem in Elements)
			{

				bool showelement = false;
				int ybottom = tmpelem.OffsetY + tmpelem.Height;

				if (((!printing) && ((ybottom >= dsply) && (tmpelem.OffsetY < dsplymax))) ||
					(printing && ((ybottom >= dsply) && (ybottom < dsplymax))))
				{
					showelement = true;
				}

				if (showelement)
				{
					endy = tmpelem.PaintElement(gfx, displayrect, origintranslate, printing);
				}
				else if (tmpelem.OffsetY >= dsplymax)
				{
					break;
				}
			}

			return endy;
		}

		public void SetDocumentDimensions(Graphics gfx)
		{
			int offsety = 0;
			int maxwidth = 0;

			foreach (GraphicsElement tmpelem in Elements)
			{
				tmpelem.RecalcSize(gfx);

				int tmpxmax = tmpelem.OffsetX + tmpelem.Width;
				if (maxwidth < tmpxmax)
					maxwidth = tmpxmax;

				tmpelem.UpdateLocation(tmpelem.OffsetX, offsety);
				offsety += tmpelem.Height;
			}

			Width = maxwidth;
			Height = offsety;
		}

		public void SetElementOffsets(Graphics gfx)
		{
			int offsety = 0;
			int maxwidth = 0;

			foreach (GraphicsElement tmpelem in Elements)
			{
				tmpelem.UpdateLocation(tmpelem.OffsetX, offsety);

				int tmpxmax = tmpelem.OffsetX + tmpelem.Width;
				if (maxwidth < tmpxmax)
					maxwidth = tmpxmax;

				offsety += tmpelem.Height;
			}

			Width = maxwidth;
			Height = offsety;
		}
	}
}
