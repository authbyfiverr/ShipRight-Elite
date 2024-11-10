using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipRight
{
	public static class ColorExtensions
	{
		public static bool ToleranceEquals(this Color color1, Color color2, int tolerance = 2)
		{
			int redDifference = Math.Abs(color1.R - color2.R);
			int greenDifference = Math.Abs(color1.G - color2.G);
			int blueDifference = Math.Abs(color1.B - color2.B);

			return redDifference <= tolerance && greenDifference <= tolerance && blueDifference <= tolerance;
		}

		public static bool ToleranceEquals(this Color color1, int r, int g, int b, int tolerance = 2)
		{
			var color2 = Color.FromArgb(r, g, b);
			int redDifference = Math.Abs(color1.R - color2.R);
			int greenDifference = Math.Abs(color1.G - color2.G);
			int blueDifference = Math.Abs(color1.B - color2.B);

			return redDifference <= tolerance && greenDifference <= tolerance && blueDifference <= tolerance;
		}
	}
}
