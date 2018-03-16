/************************************************************************************

	Project:
		GWHCAD

	File:
		GraphicsHandlers.cs

	Description:
		This souce file contains the graphics class for calculating and handling 3D
		projections.

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
	public class Graphics3D
	{
		#region Private Static Variables
		private static Point3D[] lProjectionMatrix = new Point3D[]
		{
			new Point3D(1.0F, 0.0F, 0.0F, 0.0F),
			new Point3D(0.0F, 1.0F, 0.0F, 0.0F),
			new Point3D(0.0F, 0.0F, 0.0F, 0.0F),
			new Point3D(0.0F, 0.0F, 0.0F, 1.0F)
		};
		#endregion

		#region Public Methods for Transforming Points
		public static void TransformDesignElement(double phi, double theta, DesignElement derect, double zbaseoffset)
		{
			// sanity check
			if (derect != null)
			{
				Point3D[] trimetric;                // temporary array for the trimetric transform
				int tmpndx = 0;

				// set up trimetric parameters
				trimetric = lGetTrimetricArray(phi, theta);

				// produce the temporary figure and the projection
				for (tmpndx = 0; tmpndx < derect.Points.Length; tmpndx++)
				{
					derect.TempFigurePoints[tmpndx].X = lTransformPointX(derect.Points[tmpndx], zbaseoffset, trimetric);
					derect.TempFigurePoints[tmpndx].Y = lTransformPointY(derect.Points[tmpndx], zbaseoffset, trimetric);
					derect.TempFigurePoints[tmpndx].Z = lTransformPointZ(derect.Points[tmpndx], zbaseoffset, trimetric);
					derect.TempFigurePoints[tmpndx].M = lTransformPointM(derect.Points[tmpndx], zbaseoffset, trimetric);

					derect.ProjectedPoints[tmpndx].X = lProjectPointX(derect.TempFigurePoints[tmpndx]);
					derect.ProjectedPoints[tmpndx].Y = lProjectPointY(derect.TempFigurePoints[tmpndx]);
					derect.ProjectedPoints[tmpndx].Z = lProjectPointZ(derect.TempFigurePoints[tmpndx]);
					derect.ProjectedPoints[tmpndx].M = lProjectPointM(derect.TempFigurePoints[tmpndx]);
				}

				derect.UpdateProjectedPlanes();
			}
		}

		public static Point3D TransformSinglePoint(double phi, double theta, Point3D inpt, double zbaseoffset)
		{
			Point3D[] figure_tmp;               // temporary array for figure
			Point3D[] trimetric;                // temporary array for the trimetric transform
			Point3D outpt = new Point3D(0.0F, 0.0F, 0.0F, 0.0F);

			// sanity check
			if (inpt == null)
				return null;

			// allocate the arrays needed
			figure_tmp = new Point3D[1];
			figure_tmp[0] = new Point3D();

			// set up trimetric parameters
			trimetric = lGetTrimetricArray(phi, theta);

			// produce the temporary figure
			figure_tmp[0].X = lTransformPointX(inpt, zbaseoffset, trimetric);
			figure_tmp[0].Y = lTransformPointY(inpt, zbaseoffset, trimetric);
			figure_tmp[0].Z = lTransformPointZ(inpt, zbaseoffset, trimetric);
			figure_tmp[0].M = lTransformPointM(inpt, zbaseoffset, trimetric);

			// produce the projection
			outpt.X = lProjectPointX(figure_tmp[0]);
			outpt.Y = lProjectPointY(figure_tmp[0]);
			outpt.Z = lProjectPointZ(figure_tmp[0]);
			outpt.M = lProjectPointM(figure_tmp[0]);

			// return the projected point
			return outpt;
		}
		#endregion

		#region Private Methods for Transforming Points
		private static double lTransformPointX(Point3D inpoint, double zbaseoffset, Point3D[] trimetric)
		{
			double retval = (inpoint.X * trimetric[0].X) +
							(inpoint.Y * trimetric[1].X) +
							((inpoint.Z + zbaseoffset) * trimetric[2].X) +
							(inpoint.M * trimetric[3].X);
			return retval;
		}

		private static double lTransformPointY(Point3D inpoint, double zbaseoffset, Point3D[] trimetric)
		{
			double retval = (inpoint.X * trimetric[0].Y) +
							(inpoint.Y * trimetric[1].Y) +
							((inpoint.Z + zbaseoffset) * trimetric[2].Y) +
							(inpoint.M * trimetric[3].Y);
			return retval;
		}

		private static double lTransformPointZ(Point3D inpoint, double zbaseoffset, Point3D[] trimetric)
		{
			double retval = (inpoint.X * trimetric[0].Z) +
							(inpoint.Y * trimetric[1].Z) +
							((inpoint.Z + zbaseoffset) * trimetric[2].Z) +
							(inpoint.M * trimetric[3].Z);
			return retval;
		}

		private static double lTransformPointM(Point3D inpoint, double zbaseoffset, Point3D[] trimetric)
		{
			double retval = (inpoint.X * trimetric[0].M) +
							(inpoint.Y * trimetric[1].M) +
							((inpoint.Z + zbaseoffset) * trimetric[2].M) +
							(inpoint.M * trimetric[3].M);
			return retval;
		}
		#endregion

		#region Private Methods for Projecting Points
		private static double lProjectPointX(Point3D inpoint)
		{
			double retval = (inpoint.X * lProjectionMatrix[0].X) +
							(inpoint.Y * lProjectionMatrix[1].X) +
							(inpoint.Z * lProjectionMatrix[2].X) +
							(inpoint.M * lProjectionMatrix[3].X);
			return retval;
		}

		private static double lProjectPointY(Point3D inpoint)
		{
			double retval = (inpoint.X * lProjectionMatrix[0].Y) +
							(inpoint.Y * lProjectionMatrix[1].Y) +
							(inpoint.Z * lProjectionMatrix[2].Y) +
							(inpoint.M * lProjectionMatrix[3].Y);
			return retval;
		}

		private static double lProjectPointZ(Point3D inpoint)
		{
			double retval = (inpoint.X * lProjectionMatrix[0].Z) +
							(inpoint.Y * lProjectionMatrix[1].Z) +
							(inpoint.Z * lProjectionMatrix[2].Z) +
							(inpoint.M * lProjectionMatrix[3].Z);
			return retval;
		}

		private static double lProjectPointM(Point3D inpoint)
		{
			double retval = (inpoint.X * lProjectionMatrix[0].M) +
							(inpoint.Y * lProjectionMatrix[1].M) +
							(inpoint.Z * lProjectionMatrix[2].M) +
							(inpoint.M * lProjectionMatrix[3].M);
			return retval;
		}
		#endregion

		#region Private Methods - Miscellaneous
		private static Point3D[] lGetTrimetricArray(double phi, double theta)
		{
			Point3D[] trimetric = new Point3D[4];

			trimetric[0] = new Point3D(Math.Cos(phi),	Math.Sin(phi) * Math.Sin(theta),			0.0f - (Math.Sin(phi) * Math.Cos(theta)),	0.0f);
			trimetric[1] = new Point3D(Math.Sin(phi),	0.0f - (Math.Cos(phi) * Math.Sin(theta)),	Math.Cos(phi) * Math.Cos(theta),			0.0f);
			trimetric[2] = new Point3D(0.0f,			Math.Cos(theta),							Math.Sin(theta),							0.0f);
			trimetric[3] = new Point3D(0.0f,			0.0f,										0.0f,										1.0f);

			return trimetric;
		}
		#endregion
	}
}
