/************************************************************************************

	Project:
		GWHCAD

	File:
		Common.cs

	Description:
		This souce file contains common classes for handling data.

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
	public class Point3D
	{
		public double X = 0.0F;
		public double Y = 0.0F;
		public double Z = 0.0F;
		public double M = 0.0F;

		public Point3D()
		{
			// do nothing
		}

		public Point3D(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public Point3D(double x, double y, double z, double m)
		{
			X = x;
			Y = y;
			Z = z;
			M = m;
		}

		public void SetPoint(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public void SetPoint(double x, double y, double z, double m)
		{
			X = x;
			Y = y;
			Z = z;
			M = m;
		}

		public void SetPoint(Point3D point)
		{
			X = point.X;
			Y = point.Y;
			Z = point.Z;
			M = point.M;
		}
	}

	public class RectPlane
	{
		public Point3D[] Points = new Point3D[4];

		public RectPlane()
		{
			Points[0] = new Point3D();
			Points[1] = new Point3D();
			Points[2] = new Point3D();
			Points[3] = new Point3D();
		}

		public RectPlane(Point3D p1, Point3D p2, Point3D p3, Point3D p4)
		{
			Points[0].SetPoint(p1);
			Points[1].SetPoint(p2);
			Points[2].SetPoint(p3);
			Points[3].SetPoint(p4);
		}

		public void SetPlane(RectPlane p)
		{
			Points[0].SetPoint(p.Points[0]);
			Points[1].SetPoint(p.Points[1]);
			Points[2].SetPoint(p.Points[2]);
			Points[3].SetPoint(p.Points[3]);
		}

		public void SetPlane(Point3D p1, Point3D p2, Point3D p3, Point3D p4)
		{
			Points[0].SetPoint(p1);
			Points[1].SetPoint(p2);
			Points[2].SetPoint(p3);
			Points[3].SetPoint(p4);
		}
	}
}
