using LockedBitmapUtil;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipRight
{
	internal class ToleranceColorComparator : IColorComparator
	{
		private readonly int _tolerance;

		public ToleranceColorComparator(int tolerance)
		{
			_tolerance = tolerance;
		}

		public bool IsSame(Color c1, Color c2)
		{
			return Math.Abs(c1.R - c2.R) <= _tolerance
				   && Math.Abs(c1.G - c2.G) <= _tolerance
				   && Math.Abs(c1.B - c2.B) <= _tolerance
				   && Math.Abs(c1.A - c2.A) <= _tolerance;

		}
	}
}
