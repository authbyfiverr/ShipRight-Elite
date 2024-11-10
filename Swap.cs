using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipRight
{
	internal class Swap
	{
		public Point Tile1 { get; set; }
		public Point Tile2 { get; set; }
		public Tile Tile1Type { get; set; }
		public Tile Tile2Type { get; set; }
		
		public int[][] StartingBoard { get; set; }
		public int[][] SwappedBoard { get; set; }

		public Swap(Point tile1, Point tile2, Tile tile1Type, Tile tile2Type, int[][] startingBoard, int[][] swappedBoard)
		{
			Tile1 = tile1;
			Tile2 = tile2;
			Tile1Type = tile1Type;
			Tile2Type = tile2Type;
			StartingBoard = startingBoard;
			SwappedBoard = swappedBoard;
		}
	}
}
