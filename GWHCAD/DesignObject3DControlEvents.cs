using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GWHCAD
{
	public class ControlAngleChangeArgs
	{
		public double Theta = 0.0F;
		public double Phi = 0.0F;

		public ControlAngleChangeArgs(double phi, double theta)
		{
			Theta = theta;
			Phi = phi;
		}
	}

	public class ControlScaleChangeArgs
	{
		public double ScaleValue = 0.0F;

		public ControlScaleChangeArgs(double scalevalue)
		{
			ScaleValue = scalevalue;
		}
	}

	public delegate void ControlAngleChangeCallback(object caller, ControlAngleChangeArgs angleargs);
	public delegate void ControlScalingChangeCallback(object caller, ControlScaleChangeArgs scaleargs);
}
