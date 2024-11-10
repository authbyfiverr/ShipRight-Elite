using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipRight
{
    internal class Connection
    {
	    public Point Point1;
	    public Point Point2;

	    public Connection(Point point1, Point point2)
	    {
		    Point1 = point1;
		    Point2 = point2;
	    }
    }
}
