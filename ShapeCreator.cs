using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using GameOverlay.Drawing;
using Point = System.Drawing.Point;

namespace ShipRight
{
	internal class ShapeCreator
	{

		/*
		public static void GetPerimeter(int[][] shape, List<Point> shapePixelPoints)
		{
			int rows = shape.Length;
			int cols = shape[0].Length;

			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < cols; j++)
				{
					if (shape[i][j] != 1 && IsPerimeterCell(shape, i, j))
					{
						perimeter.Add((i, j));
					}
				}
			}
			return perimeter;
		}*/

		public static bool IsPerimeterCell(int[][] shape, int x, int y, PerimeterTile currentPerimeterTile)
		{
			bool isPerimeterCell = false;

			int rows = shape.Length;
			int cols = shape[0].Length;

			int[] dx = { -1, 0, 1, 0 };
			int[] dy = { 0, 1, 0, -1 };

			for (int i = 0; i < 4; i++)
			{
				int newX = x + dx[i];
				int newY = y + dy[i];

				//Left, Down, Right, Up
				if (newX >= 0 && newX < cols && 
				    newY >= 0 && newY < rows)
				{
					if (shape[newY][newX] == 0)
					{
						switch (i)
						{
							case 0:
								currentPerimeterTile.Edges.Left = true;
								break;
							case 1:
								currentPerimeterTile.Edges.Down = true;
								break;
							case 2:
								currentPerimeterTile.Edges.Right = true;
								break;
							case 3:
								currentPerimeterTile.Edges.Up = true;
								break;
						}

						isPerimeterCell = true;
					}
				}
				else
				{
					switch (i)
					{
						case 0:
							currentPerimeterTile.Edges.Left = true;
							break;
						case 1:
							currentPerimeterTile.Edges.Down = true;
							break;
						case 2:
							currentPerimeterTile.Edges.Right = true;
							break;
						case 3:
							currentPerimeterTile.Edges.Up = true;
							break;
					}
					isPerimeterCell = true;
				}
			}
			return isPerimeterCell;
		}


		public static void BuildEdges(ShapePerimeter currentShape)
		{
			for (int i = 0; i < currentShape.Shape.Length; i++)
			{
				for (int j = 0; j < currentShape.Shape[i].Length; j++)
				{
					if (currentShape.Shape[j][i] == 0) continue;

					var curTile = currentShape.TilesArray[j][i];
					var originPoint = new Point(curTile.PixelOriginPoint.X, curTile.PixelOriginPoint.Y);
					if (curTile.Edges.Up)
					{
						currentShape.EdgeLines.Add(new Line(new GameOverlay.Drawing.Point(originPoint.X, originPoint.Y+2),
																new GameOverlay.Drawing.Point(originPoint.X + 60, originPoint.Y+2)));
					}
					if (curTile.Edges.Right)
					{
						currentShape.EdgeLines.Add(new Line(new GameOverlay.Drawing.Point(originPoint.X + 60-2, originPoint.Y),
																new GameOverlay.Drawing.Point(originPoint.X + 60-2, originPoint.Y + 60)));
					}
					if (curTile.Edges.Left)
					{
						currentShape.EdgeLines.Add(new Line(new GameOverlay.Drawing.Point(originPoint.X+2, originPoint.Y),
																new GameOverlay.Drawing.Point(originPoint.X+2, originPoint.Y + 60)));
					}
					if (curTile.Edges.Down)
					{
						currentShape.EdgeLines.Add(new Line(new GameOverlay.Drawing.Point(originPoint.X, originPoint.Y + 60-2),
																new GameOverlay.Drawing.Point(originPoint.X + 60, originPoint.Y + 60-2)));
					}
				}
			}
		}
	}

	public class ShapePerimeter
	{
		public List<Line> EdgeLines { get; set; }
		public int[][] Shape { get; set; }
		public PerimeterTile[][] TilesArray { get; set; }

		public ShapePerimeter(int[][] shape)
		{
			EdgeLines = new List<Line>();
			Shape = shape;
			TilesArray = new PerimeterTile[shape.Length][];
			for (int i = 0; i < shape.Length; i++)
			{
				TilesArray[i] = new PerimeterTile[shape[i].Length];
			}
		}

		public ShapePerimeter Clone()
		{
			int[][] clonedShape = this.Shape.DeepClone();
			ShapePerimeter clonedPerimeter = new ShapePerimeter(clonedShape);
			clonedPerimeter.EdgeLines = CloneEdgeLines(EdgeLines);
			clonedPerimeter.TilesArray= this.TilesArray.DeepClone();
			return clonedPerimeter;
		}

		private int[][] CloneShapeArray(int[][] shape)
		{
			int[][] clonedShape = new int[shape.Length][];
			for (int i = 0; i < shape.Length; i++)
			{
				clonedShape[i] = new int[shape[i].Length];
				Array.Copy(shape[i], clonedShape[i], shape[i].Length);
			}
			return clonedShape;
		}

		private List<Line> CloneEdgeLines(List<Line> edgeLines)
		{
			List<Line> clonedEdgeLines = new List<Line>(edgeLines.Count);
			foreach (Line line in edgeLines)
			{
				clonedEdgeLines.Add(new Line(line.Start, line.End));
			}
			return clonedEdgeLines;
		}

		private PerimeterTile[][] CloneTilesArray(PerimeterTile[][] tilesArray)
		{
			PerimeterTile[][] clonedTilesArray = new PerimeterTile[tilesArray.Length][];
			for (int i = 0; i < tilesArray.Length; i++)
			{
				clonedTilesArray[i] = new PerimeterTile[tilesArray[i].Length];
				for (int j = 0; j < tilesArray[i].Length; j++)
				{
					clonedTilesArray[i][j] = tilesArray[i][j].Clone();
				}
			}
			return clonedTilesArray;
		}
	}

	public class PerimeterTile
	{
		public OpenEdges Edges { get; set; }
		public int ShapeArrayX { get; set; }
		public int ShapeArrayY { get; set; }
		public Point PixelOriginPoint { get; set; }

		public PerimeterTile(int shapeArrayX, int shapeArrayY, Point pixelOriginPoint)
		{
			Edges = new OpenEdges();
			ShapeArrayX = shapeArrayX;
			ShapeArrayY = shapeArrayY;
			PixelOriginPoint = pixelOriginPoint;
		}

		public PerimeterTile Clone()
		{
			PerimeterTile clonedTile = new PerimeterTile(ShapeArrayX, ShapeArrayY,
				new Point(PixelOriginPoint.X, PixelOriginPoint.Y));
			clonedTile.Edges = Edges.Clone();
			return clonedTile;
		}

		public class OpenEdges
		{
			public bool Left { get; set; } = false;
			public bool Right { get; set; } = false;
			public bool Up { get; set; } = false;
			public bool Down { get; set; } = false;

			public OpenEdges Clone()
			{
				OpenEdges clonedEdges = new OpenEdges
				{
					Left = this.Left,
					Right = this.Right,
					Up = this.Up,
					Down = this.Down
				};
				return clonedEdges;
			}
		}

	}
}
