/************************************************************************************

	Project:
		GWHCAD

	File:
		DesignObject.cs

	Description:
		This souce file contains a standard string operations class.

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

namespace GWHCAD
{
	public class DesignPoint
	{
		public enum OriginType
		{
			Midpoint,
			Edge
		}

		public OriginType Origin = OriginType.Midpoint;
		public double Direction = 1.0F;
		public double Value = 0.0F;

		public DesignPoint()
		{
			//
		}

		public DesignPoint(string inputtext)
		{
			UpdateFromText(inputtext);
		}

		public string GetTextForParameters()
		{
			string txtparams = "";

			txtparams = (Origin == OriginType.Edge ? "E" : "M");
			txtparams += (Direction < 0.0F ? "-" : "+");
			txtparams += Value.ToString();

			return txtparams;
		}

		public void SetFromDesignPoint(DesignPoint srcpt)
		{
			Origin = srcpt.Origin;
			Direction = srcpt.Direction;
			Value = srcpt.Value;
		}

		public bool UpdateFromText(string inputtext)
		{
			bool retval = true;

			try
			{
				if (inputtext.Length >= 3)
				{
					Direction = (inputtext[1] == '-' ? -1.0F : 1.0F);

					if (char.IsLetter(inputtext[2]))
					{
						Origin = OriginType.Edge;
						Value = 0.0F;
					}
					else
					{
						Origin = (inputtext[0] == 'E' ? OriginType.Edge : OriginType.Midpoint);
						Value = Convert.ToDouble(inputtext.Substring(2));
					}
				}
				else
					retval = false;
			}
			catch (Exception /*ex*/)
			{
				retval = false;
			}

			return retval;
		}
	}

	public class DesignAttachmentParameters
	{

		public DesignPoint AttX = new DesignPoint();
		public DesignPoint AttY = new DesignPoint();
		public DesignPoint AttZ = new DesignPoint();
		public DesignPoint OwnX = new DesignPoint();
		public DesignPoint OwnY = new DesignPoint();
		public DesignPoint OwnZ = new DesignPoint();
		public DesignElement Attachment = null;

		public string AttachmentName
		{
			get { return lGetAttachmentName(); }
		}

		private string lAttachmentName = "";

		public DesignAttachmentParameters()
		{
			// do nothing
		}

		public DesignAttachmentParameters(string attachmentname)
		{
			lAttachmentName = attachmentname;
		}

		public double GetAttachmentPointX()
		{
			double atx = 0.0F;

			if (Attachment != null)
			{
				double atxval = AttX.Value * AttX.Direction;

				atx = Attachment.Midpoint.X;

				if (AttX.Origin == DesignPoint.OriginType.Midpoint)
					atx += atxval;
				else
					atx += ((Attachment.GetDimensionByType(DesignElement.DimensionTypes.X) / 2.0F) * AttX.Direction) - atxval;
			}

			return atx;
		}

		public double GetAttachmentPointY()
		{
			double aty = 0.0F;

			if (Attachment != null)
			{
				double atyval = AttY.Value * AttY.Direction;

				aty = Attachment.Midpoint.Y;

				if (AttY.Origin == DesignPoint.OriginType.Midpoint)
					aty += atyval;
				else
					aty += ((Attachment.GetDimensionByType(DesignElement.DimensionTypes.Y) / 2.0F) * AttY.Direction) - atyval;
			}

			return aty;
		}

		public double GetAttachmentPointZ()
		{
			double atz = 0.0F;

			if (Attachment != null)
			{
				double atzval = AttZ.Value * AttZ.Direction;

				atz = Attachment.Midpoint.Z;

				if (AttZ.Origin == DesignPoint.OriginType.Midpoint)
					atz += atzval;
				else
					atz += ((Attachment.GetDimensionByType(DesignElement.DimensionTypes.Z) / 2.0F) * AttZ.Direction) - atzval;
			}

			return atz;
		}

		public double GetOwnPointX(double dimx)
		{
			double atx = 0.0F;
			double atxval = OwnX.Value * OwnX.Direction;

			if (OwnX.Origin == DesignPoint.OriginType.Midpoint)
				atx = atxval;
			else
				atx = ((dimx / 2.0F) * OwnX.Direction) - atxval;

			return atx;
		}

		public double GetOwnPointY(double dimy)
		{
			double aty = 0.0F;
			double atyval = OwnY.Value * OwnY.Direction;

			if (OwnY.Origin == DesignPoint.OriginType.Midpoint)
				aty = atyval;
			else
				aty = ((dimy / 2.0F) * OwnY.Direction) - atyval;

			return aty;
		}

		public double GetOwnPointZ(double dimz)
		{
			double atz = 0.0F;
			double atzval = OwnZ.Value * OwnZ.Direction;

			if (OwnZ.Origin == DesignPoint.OriginType.Midpoint)
				atz = atzval;
			else
				atz = ((dimz / 2.0F) * OwnZ.Direction) - atzval;

			return atz;
		}

		public bool SetAttachmentParametersFromStrings(string[] attachstrs)
		{
			bool retval = false;

			if ((attachstrs != null) && (attachstrs.Length == 3))
			{
				if (AttX.UpdateFromText(attachstrs[0]) && AttY.UpdateFromText(attachstrs[1]) && AttZ.UpdateFromText(attachstrs[2]))
					retval = true;
			}

			return retval;
		}

		public void SetFromAttachmentPoint(DesignAttachmentParameters srcpt)
		{
			AttX.SetFromDesignPoint(srcpt.AttX);
			AttY.SetFromDesignPoint(srcpt.AttY);
			AttZ.SetFromDesignPoint(srcpt.AttZ);
			OwnX.SetFromDesignPoint(srcpt.OwnX);
			OwnY.SetFromDesignPoint(srcpt.OwnY);
			OwnZ.SetFromDesignPoint(srcpt.OwnZ);
			Attachment = srcpt.Attachment;
		}

		public bool SetOwnParametersFromStrings(string[] ownstrs)
		{
			bool retval = false;

			if ((ownstrs != null) && (ownstrs.Length == 3))
			{
				if (OwnX.UpdateFromText(ownstrs[0]) && OwnY.UpdateFromText(ownstrs[1]) && OwnZ.UpdateFromText(ownstrs[2]))
					retval = true;
			}

			return retval;
		}

		private string lGetAttachmentName()
		{
			return (Attachment == null ? lAttachmentName : Attachment.Name);
		}
	}

	public class DesignElement
	{
		public enum ElementTypes
		{
			Undefined,
			Rect
		}

		public enum DimensionTypes
		{
			X,
			Y,
			Z
		}

		public string Name = "";
		public ElementTypes ElementType = ElementTypes.Undefined;
		public Point3D[] Points = new Point3D[0];
		public Point3D[] TempFigurePoints = new Point3D[0];
		public Point3D[] ProjectedPoints = new Point3D[0];

		public Point3D Midpoint
		{
			get { return lMidpoint; }
			set { lSetFromMidpoint(value); }
		}
		public DesignAttachmentParameters AttachmentPoint
		{
			get { return lAttachmentPoint; }
			set { lSetFromAttachmentPoint(value); }
		}
		public double DimensionX
		{
			get { return lDimensionX; }
			set { lSetDimensionX(value); }
		}
		public double DimensionY
		{
			get { return lDimensionY; }
			set { lSetDimensionY(value); }
		}
		public double DimensionZ
		{
			get { return lDimensionZ; }
			set { lSetDimensionZ(value); }
		}
		public RectPlane[] Planes
		{
			get { return lPlanes; }
		}
		public RectPlane[] ProjectedPlanes
		{
			get { return lProjectedPlanes; }
		}

		public virtual double GetElementDimensionX() { return 0.0F; }
		public virtual double GetElementDimensionY() { return 0.0F; }
		public virtual double GetElementDimensionZ() { return 0.0F; }

		protected Point3D lMidpoint = new Point3D();
		protected DesignAttachmentParameters lAttachmentPoint = null;
		protected RectPlane[] lPlanes = new RectPlane[0];
		protected RectPlane[] lProjectedPlanes = new RectPlane[0];
		protected double lDimensionX = 0.0F;
		protected double lDimensionY = 0.0F;
		protected double lDimensionZ = 0.0F;

		public virtual DesignElement Duplicate()
		{
			DesignElement newde = new DesignElement();

			newde.Name = Name;
			newde.ElementType = ElementType;
			newde.lMidpoint.SetPoint(lMidpoint);
			newde.lAttachmentPoint.SetFromAttachmentPoint(lAttachmentPoint);

			return newde;
		}

		public double GetDimensionByType(DimensionTypes dimension)
		{
			double dimval = 0.0F;

			switch (dimension)
			{
				case DimensionTypes.X:
					dimval = GetElementDimensionX();
					break;
				case DimensionTypes.Y:
					dimval = GetElementDimensionY();
					break;
				case DimensionTypes.Z:
					dimval = GetElementDimensionZ();
					break;
			}

			return dimval;
		}

		public double GetRelativeDimension(DimensionTypes dimension, string text)
		{
			double relval = 0.0;

			if (text.Length >= 3)
			{
				char signval = text[1];
				string valstr = text.Substring(2);
				double dimhalf = GetDimensionByType(dimension) / 2.0F;
				double signmult = (signval == '-' ? -1.0F : 1.0F);

				if (valstr.Equals("D"))
					relval = dimhalf * signmult;
				else
				{
					char relto = text[0];

					if (relto == 'E')
						relval = dimhalf - (signmult * Convert.ToDouble(valstr));
					else if (relto == 'M')
						relval = signmult * Convert.ToDouble(valstr);
				}
			}

			return relval;
		}

		public virtual void UpdatePoints() { }

		public void UpdateProjectedPlanes()
		{
			lSetProjectedPlanes();
		}

		protected virtual void lRecalcPoints() { }

		private void lSetDimensionX(double x)
		{
			lDimensionX = x;
			lRecalcPoints();
		}

		private void lSetDimensionY(double y)
		{
			lDimensionY = y;
			lRecalcPoints();
		}

		private void lSetDimensionZ(double z)
		{
			lDimensionZ = z;
			lRecalcPoints();
		}

		protected void lSetFromMidpoint(Point3D point)
		{
			lMidpoint.SetPoint(point);
			lRecalcPoints();
		}

		protected void lSetFromMidpoint(double x, double y, double z)
		{
			lMidpoint.SetPoint(x, y, z);
			lRecalcPoints();
		}

		protected void lSetFromAttachmentPoint(DesignAttachmentParameters element)
		{
			lAttachmentPoint = element;
			lRecalcPoints();
		}

		protected virtual void lSetProjectedPlanes() { }
	}

	public class DesignElementRect : DesignElement
	{

		public DesignElementRect()
		{
			int tmpndx;

			ElementType = ElementTypes.Rect;
			Points = new Point3D[8];
			TempFigurePoints = new Point3D[8];
			ProjectedPoints = new Point3D[8];
			lPlanes = new RectPlane[6];
			lProjectedPlanes = new RectPlane[6];

			for (tmpndx = 0; tmpndx < Points.Length; tmpndx++)
			{
				Points[tmpndx] = new Point3D();
				TempFigurePoints[tmpndx] = new Point3D();
				ProjectedPoints[tmpndx] = new Point3D();
			}

			for (tmpndx = 0; tmpndx < lPlanes.Length; tmpndx++)
			{
				lPlanes[tmpndx] = new RectPlane();
				lProjectedPlanes[tmpndx] = new RectPlane();
			}
		}

		public override DesignElement Duplicate()
		{
			DesignElementRect newde = new DesignElementRect();
			int ndx = 0;

			newde.Name = Name;
			newde.lMidpoint.SetPoint(lMidpoint);
			newde.lDimensionX = lDimensionX;
			newde.lDimensionY = lDimensionY;
			newde.lDimensionZ = lDimensionZ;

			for (ndx = 0; ndx < Points.Length; ndx++)
			{
				newde.Points[ndx].SetPoint(Points[ndx]);
				newde.TempFigurePoints[ndx].SetPoint(TempFigurePoints[ndx]);
				newde.ProjectedPoints[ndx].SetPoint(ProjectedPoints[ndx]);
			}
			newde.lSetPlanes();
			newde.lSetProjectedPlanes();

			if (lAttachmentPoint != null)
			{
				newde.lAttachmentPoint = new DesignAttachmentParameters();
				newde.lAttachmentPoint.SetFromAttachmentPoint(lAttachmentPoint);
			}

			return newde;
		}

		public override double GetElementDimensionX()
		{
			return lDimensionX;
		}

		public override double GetElementDimensionY()
		{
			return lDimensionY;
		}

		public override double GetElementDimensionZ()
		{
			return lDimensionZ;
		}

		public override void UpdatePoints()
		{
			lRecalcPoints();
		}

		protected override void lRecalcPoints()
		{
			if ((lDimensionX != 0.0F) && (lDimensionY != 0.0F) && (lDimensionZ != 0.0F))
			{
				double x;
				double y;
				double z;
				double halfx = lDimensionX / 2.0F;
				double halfy = lDimensionY / 2.0F;
				double halfz = lDimensionZ / 2.0F;

				if (lAttachmentPoint != null)
				{
					DesignElementRect attrect = (DesignElementRect)lAttachmentPoint.Attachment;
					double ownx = lAttachmentPoint.GetOwnPointX(lDimensionX);
					double owny = lAttachmentPoint.GetOwnPointY(lDimensionY);
					double ownz = lAttachmentPoint.GetOwnPointZ(lDimensionZ);
					double attx = lAttachmentPoint.GetAttachmentPointX();
					double atty = lAttachmentPoint.GetAttachmentPointY();
					double attz = lAttachmentPoint.GetAttachmentPointZ();
					double midx = attx - ownx;
					double midy = atty - owny;
					double midz = attz - ownz;

					lMidpoint.SetPoint(midx, midy, midz);
				}

				x = lMidpoint.X;
				y = lMidpoint.Y;
				z = lMidpoint.Z;

				Points[0].SetPoint(x + halfx, y + halfy, z + halfz);
				Points[1].SetPoint(x - halfx, y + halfy, z + halfz);
				Points[2].SetPoint(x - halfx, y - halfy, z + halfz);
				Points[3].SetPoint(x + halfx, y - halfy, z + halfz);
				Points[4].SetPoint(x + halfx, y + halfy, z - halfz);
				Points[5].SetPoint(x - halfx, y + halfy, z - halfz);
				Points[6].SetPoint(x - halfx, y - halfy, z - halfz);
				Points[7].SetPoint(x + halfx, y - halfy, z - halfz);

				lSetPlanes();
			}
		}

		private void lSetPlanes()
		{
			lPlanes[0].SetPlane(Points[0], Points[1], Points[2], Points[3]);
			lPlanes[1].SetPlane(Points[4], Points[5], Points[6], Points[7]);
			lPlanes[2].SetPlane(Points[0], Points[1], Points[5], Points[4]);
			lPlanes[3].SetPlane(Points[1], Points[2], Points[6], Points[5]);
			lPlanes[4].SetPlane(Points[2], Points[3], Points[7], Points[6]);
			lPlanes[5].SetPlane(Points[0], Points[3], Points[7], Points[4]);
		}

		protected override void lSetProjectedPlanes()
		{
			lProjectedPlanes[0].SetPlane(ProjectedPoints[0], ProjectedPoints[1], ProjectedPoints[2], ProjectedPoints[3]);
			lProjectedPlanes[1].SetPlane(ProjectedPoints[4], ProjectedPoints[5], ProjectedPoints[6], ProjectedPoints[7]);
			lProjectedPlanes[2].SetPlane(ProjectedPoints[0], ProjectedPoints[1], ProjectedPoints[5], ProjectedPoints[4]);
			lProjectedPlanes[3].SetPlane(ProjectedPoints[1], ProjectedPoints[2], ProjectedPoints[6], ProjectedPoints[5]);
			lProjectedPlanes[4].SetPlane(ProjectedPoints[2], ProjectedPoints[3], ProjectedPoints[7], ProjectedPoints[6]);
			lProjectedPlanes[5].SetPlane(ProjectedPoints[0], ProjectedPoints[3], ProjectedPoints[7], ProjectedPoints[4]);
		}
	}

	public class DesignObject
	{
		public enum Units
		{
			Undefined,
			Inch,
			Foot,
			Millimeter,
			Centimeter,
			Meter
		}

		public string Name = "";
		public Units UnitType = Units.Undefined;
		public List<DesignElement> Elements = new List<DesignElement>();

		public DesignObject()
		{
			// do nothing
		}

		public DesignElement DuplicateElement(string srcname, string newname)
		{
			DesignElement newelem = null;
			DesignElement srcelem = GetDesignElementByName(srcname);

			if (srcelem != null)
			{
				//newelem = 
			}

			return newelem;
		}

		public DesignElement GetDesignElementByName(string name)
		{
			DesignElement delem = null;

			if (Elements.Count > 0)
			{
				foreach (DesignElement tmpdelem in Elements)
				{
					if (tmpdelem.Name.Equals(name))
					{
						delem = tmpdelem;
						break;
					}
				}
			}

			return delem;
		}

		public Point3D GetMaxCoordinates()
		{
			Point3D maxpt = new Point3D(0.0F, 0.0F, 0.0F);

			if (Elements.Count > 0)
			{
				foreach (DesignElement tmpdelem in Elements)
				{
					double dimx = tmpdelem.Midpoint.X + (tmpdelem.GetElementDimensionX() / 2.0F);
					double dimy = tmpdelem.Midpoint.Y + (tmpdelem.GetElementDimensionY() / 2.0F);
					double dimz = tmpdelem.Midpoint.Z + (tmpdelem.GetElementDimensionZ() / 2.0F);

					if (maxpt.X < dimx)
						maxpt.X = dimx;
					if (maxpt.Y < dimy)
						maxpt.Y = dimy;
					if (maxpt.Z < dimz)
						maxpt.Z = dimz;
				}
			}

			return maxpt;
		}

		public Point3D GetMinCoordinates()
		{
			Point3D maxpt = new Point3D();

			if (Elements.Count > 0)
			{
				foreach (DesignElement tmpdelem in Elements)
				{
					double dimx = tmpdelem.Midpoint.X - (tmpdelem.GetElementDimensionX() / 2.0F);
					double dimy = tmpdelem.Midpoint.Y - (tmpdelem.GetElementDimensionY() / 2.0F);
					double dimz = tmpdelem.Midpoint.Z - (tmpdelem.GetElementDimensionZ() / 2.0F);

					if (maxpt.X > dimx)
						maxpt.X = dimx;
					if (maxpt.Y > dimy)
						maxpt.Y = dimy;
					if (maxpt.Z > dimz)
						maxpt.Z = dimz;
				}
			}

			return maxpt;
		}

		public bool SaveDesignObject(string filename)
		{
			bool retval = false;
			List<string> dotext = new List<string>();

			// set header comment
			dotext.Add(";");
			dotext.Add("; " + Name);
			dotext.Add(";");
			dotext.Add("; Saved: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			dotext.Add(";");
			dotext.Add("");

			// set design object parameters
			dotext.Add("; Design object name");
			dotext.Add("OBJECTNAME=" + Name);
			dotext.Add("");
			dotext.Add("; Measurements to be displayed");
			dotext.Add("MEASURE=" + UnitType.ToString().ToUpper());

			// add each design element
			if (Elements.Count > 0)
			{
				dotext.Add("");
				dotext.Add("; Individual design elements (parts)");

				foreach (DesignElement tmpdelem in Elements)
				{
					dotext.Add("");
					dotext.Add("; Design element object: " + tmpdelem.Name);
					dotext.Add("OBJECTSTART=" + tmpdelem.ElementType.ToString());
					dotext.Add("\tNAME=" + tmpdelem.Name);
					dotext.Add("\tDIMENSIONS=" + tmpdelem.GetElementDimensionX().ToString() + "," + tmpdelem.GetElementDimensionY().ToString() + "," + tmpdelem.GetElementDimensionZ().ToString());
					if (tmpdelem.AttachmentPoint != null)
					{
						DesignAttachmentParameters attparams = tmpdelem.AttachmentPoint;

						dotext.Add("\tATTACH=" + attparams.OwnX.GetTextForParameters() + "," + attparams.OwnY.GetTextForParameters() + "," + attparams.OwnZ.GetTextForParameters() + ",\"" +
							attparams.Attachment.Name + "\"," + attparams.AttX.GetTextForParameters() + "," + attparams.AttY.GetTextForParameters() + "," + attparams.AttZ.GetTextForParameters());
					}
					else
					{
						dotext.Add("\tMIDPOINT=" + tmpdelem.Midpoint.X.ToString() + "," + tmpdelem.Midpoint.Y.ToString() + "," + tmpdelem.Midpoint.Z.ToString());
					}
					dotext.Add("OBJECTEND=");
				}
			}

			retval = FileHandler.SaveFile(filename, dotext);

			return retval;
		}

		public static DesignObject LoadDesignObject(string filename)
		{
			DesignObject dobj = null;
			string[] filetext = FileHandler.LoadFile(filename);

			if ((filetext != null) && (filetext.Length > 0))
			{
				char[] eqdelim = new char[] { '=' };
				char[] commadelim = new char[] { ',' };
				char[] quotedelim = new char[] { '"' };
				bool error = false;
				DesignElement currelement = null;
				int posndx = 0;

				dobj = new DesignObject();

				foreach (string fileline in filetext)
				{
					string linetrim = fileline.Trim();

					if ((linetrim.Length > 0) && (linetrim[0] != ';'))
					{
						string[] linetokens = linetrim.Split(eqdelim);

						if (linetokens.Length == 2)
						{
							string varname = linetokens[0].ToLower();

							if (currelement != null)
							{
								if (varname.Equals("name"))
								{
									currelement.Name = linetokens[1];
								}
								else if (varname.Equals("objectend"))
								{
									currelement = null;
								}
								else if (currelement.ElementType == DesignElement.ElementTypes.Rect)
								{
									DesignElementRect rectelem = (DesignElementRect)currelement;

									if (varname.Equals("dimensions"))
									{
										string[] dimstrs = linetokens[1].Split(commadelim);

										if (dimstrs.Length == 3)
										{
											try
											{
												rectelem.DimensionX = Convert.ToDouble(dimstrs[0]);
												rectelem.DimensionY = Convert.ToDouble(dimstrs[1]);
												rectelem.DimensionZ = Convert.ToDouble(dimstrs[2]);
												rectelem.UpdatePoints();
											}
											catch (System.FormatException /*fex*/)
											{
												error = true;
											}
										}
										else
										{
											error = true;
										}
									}
									else if (varname.Equals("attach"))
									{
										string[] namestrs = linetokens[1].Split(quotedelim);

										if (namestrs.Length == 3)
										{
											string[] ownstrs = namestrs[0].Split(commadelim, StringSplitOptions.RemoveEmptyEntries);
											string[] attrstrs = namestrs[2].Split(commadelim, StringSplitOptions.RemoveEmptyEntries);

											if ((ownstrs.Length == 3) && (attrstrs.Length == 3))
											{
												DesignAttachmentParameters dattach = new DesignAttachmentParameters(namestrs[1]);

												if (!dattach.SetAttachmentParametersFromStrings(attrstrs))
													error = true;
												else if (!dattach.SetOwnParametersFromStrings(ownstrs))
													error = true;

												rectelem.AttachmentPoint = dattach;
											}
											else
											{
												error = true;
											}
										}
										else
										{
											error = true;
										}
									}
									else if (varname.Equals("midpoint"))
									{
										string[] midstrs = linetokens[1].Split(commadelim);

										if (midstrs.Length == 3)
										{
											try
											{
												double midx = Convert.ToDouble(midstrs[0]);
												double midy = Convert.ToDouble(midstrs[1]);
												double midz = Convert.ToDouble(midstrs[2]);

												rectelem.Midpoint.SetPoint(midx, midy, midz);
												rectelem.UpdatePoints();
											}
											catch (System.FormatException /*fex*/)
											{
												error = true;
											}
										}
										else
										{
											error = true;
										}
									}
									else if (varname.Equals("position"))
									{
										string[] posstrs = linetokens[1].Split(commadelim);

										if ((posstrs.Length == 3) && (posndx < rectelem.Points.Length))
										{
											try
											{
												double midx = Convert.ToDouble(posstrs[0]);
												double midy = Convert.ToDouble(posstrs[1]);
												double midz = Convert.ToDouble(posstrs[2]);

												rectelem.Points[posndx].SetPoint(midx, midy, midz);
												posndx++;
											}
											catch (System.FormatException /*fex*/)
											{
												error = true;
											}
										}
										else
										{
											error = true;
										}
									}
									else
									{
										error = true;
									}
								}
								else
								{
									error = true;
								}
							}
							else if (varname.Equals("objectname"))
							{
								dobj.Name = linetokens[1];
							}
							else if (varname.Equals("measure"))
							{
								Units unitval;

								if (Enum.TryParse(linetokens[1], true, out unitval) && Enum.IsDefined(typeof(Units), unitval))
								{
									dobj.UnitType = unitval;
								}
								else
								{
									error = true;
								}
							}
							else if (varname.Equals("objectstart"))
							{
								if (currelement == null)
								{
									DesignElement.ElementTypes etype;

									if (Enum.TryParse(linetokens[1], true, out etype) && Enum.IsDefined(typeof(DesignElement.ElementTypes), etype))
									{
										switch (etype)
										{
											case DesignElement.ElementTypes.Rect:
												currelement = new DesignElementRect();
												break;

											default:
												error = true;
												break;
										}

										if (currelement != null)
										{
											dobj.Elements.Add(currelement);
											posndx = 0;
										}
									}
									else
									{
										error = true;
									}
								}
								else
								{
									error = true;
								}
							}
						}
						else
						{
							error = true;
						}
					}

					if (error)
					{
						break;
					}
				}

				if (error)
				{
					dobj = null;
				}
				else
				{
					// fill in any relative attachment point classes
					if (dobj.Elements.Count > 0)
					{
						List<DesignElement> attelems = new List<DesignElement>();

						// make a list of all elements that have a primary attachment
						foreach (DesignElement tmpdelem in dobj.Elements)
						{
							lUpdateAttachmentPoints(dobj, tmpdelem);
						}
					}
				}
			}

			return dobj;
		}

		public void UpdatePointsForElement(DesignElement delem)
		{
			delem.UpdatePoints();

			foreach (DesignElement tmpdelem in Elements)
			{
				if ((tmpdelem.AttachmentPoint != null) && (tmpdelem.AttachmentPoint.Attachment == delem))
				{
					UpdatePointsForElement(tmpdelem);
				}
			}
		}

		private static void lUpdateAttachmentPoints(DesignObject dobj, DesignElement delem)
		{
			if (delem.AttachmentPoint != null)
			{
				if (delem.AttachmentPoint.Attachment == null)
				{
					delem.AttachmentPoint.Attachment = dobj.GetDesignElementByName(delem.AttachmentPoint.AttachmentName);
					lUpdateAttachmentPoints(dobj, delem.AttachmentPoint.Attachment);
					delem.UpdatePoints();
				}
			}
		}
	}
}
