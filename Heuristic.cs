using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShipRight.Extensions;

namespace ShipRight
{
	internal static class Heuristic
	{

		private static readonly Random _rand = new Random();

		public static double GetHeuristic(int[][] board, int[][] goalBoard)
		{
			double manhattanDistance = 0;
			int mismatches = 25;
			double multiplier = 1;

			for (int row = 0; row < board.Length; row++)
			{
				for (int col = 0; col < board[row].Length; col++)
				{
					int tile = board[row][col];
					if (tile == 0)
						throw new ArgumentException { };
					if (tile == goalBoard[row][col])
					{
						mismatches--;
					}
					else
					{


						var goalPositions = GetPositions(goalBoard, tile);
						int minDistance = int.MaxValue;
						foreach (var (goalRow, goalCol) in goalPositions)
						{
							int distance = Math.Abs(row - goalRow) + Math.Abs(col - goalCol);
							if (distance < minDistance)
							{
								minDistance = distance;
							}
						}
						manhattanDistance += minDistance;

						double restrictionCount = 0;
						double neighbors = 0;
						foreach (var (adjRow, adjCol) in GetAdjacentTiles(row, col, board.Length, board[row].Length))
						{
							if (!CanSwapTiles(board, row, col, adjRow, adjCol))
							{
								restrictionCount++;
							}
							neighbors++;
						}
						multiplier += (Math.Pow(restrictionCount + 1, 1.5) / Math.Pow(neighbors + 1, 1.5));

					}
				}
			}

			// Weights for direct matches and Manhattan distance
			double directMatchWeight = 0.5;
			double manhattanDistanceWeight = 1;
			double linearConflictWeight = 0.05;

			// Calculate the weighted sum of direct matches, Manhattan distance, and linear conflict
			double heuristicValue = multiplier * (directMatchWeight * mismatches + manhattanDistanceWeight * manhattanDistance);

			return heuristicValue;
		}


		public static double GetHeuristic2(int[][] board, int[][] goalBoard)
		{
			var adjustedGoalBoard = goalBoard.DeepClone();
			AdjustBlockedTiles(board, goalBoard, adjustedGoalBoard);

			double manhattanDistance = 0;
			int mismatches = 25;
			double multiplier = 1;

			// Weights for direct matches and Manhattan distance
			double directMatchWeight = 0.5;
			double manhattanDistanceWeight = 1;

			for (int row = 0; row < board.Length; row++)
			{
				for (int col = 0; col < board[row].Length; col++)
				{
					int tile = board[row][col];
					if (tile == 0)
						throw new ArgumentException { };


					if (tile == goalBoard[row][col])
					{
						mismatches--;
					}
					else
					{
						
						var neighborsArray = GetNeighbors(board, row, col);
						var restrictionsCount = CountRestrictions(neighborsArray);

						if ((IsCorner(row, col, board.Length, board[0].Length) && restrictionsCount > 6) || restrictionsCount > 4)
						{
							var openOptions = FindOpenOptions(neighborsArray);
							var (closestOptionRow, closestOptionCol) = FindClosestInteger(board, row, col, openOptions);

							var requiredTile = board[closestOptionRow][closestOptionCol];

							if (adjustedGoalBoard[row][col] != requiredTile)
							{
								adjustedGoalBoard[row][col] = requiredTile;
							}
						}


						
						double restrictionCount = 0;
						double neighbors = 0;
						foreach (var (adjRow, adjCol) in GetAdjacentTiles(row, col, board.Length, board[row].Length))
						{

							if (!CanSwapTiles(board, row, col, adjRow, adjCol))
								restrictionCount++;

							neighbors++;
						}
						
						multiplier += (Math.Pow(restrictionCount + 1, 1.5) / Math.Pow(neighbors + 1, 1.5));

						var goalPositions = GetPositions(adjustedGoalBoard, tile);
						int minDistance = int.MaxValue;
						foreach (var (goalRow, goalCol) in goalPositions)
						{
							int distance = Math.Abs(row - goalRow) + Math.Abs(col - goalCol);
							if (distance < minDistance)
							{
								minDistance = distance;
							}
						}

						manhattanDistance += minDistance;
					}
				}
			}

			// Calculate the weighted sum of direct matches, Manhattan distance, and linear conflict
			double heuristicValue = multiplier * (directMatchWeight * mismatches + manhattanDistanceWeight * manhattanDistance);
			//double heuristicValue = (directMatchWeight * mismatches + manhattanDistanceWeight * manhattanDistance);

			return heuristicValue;
		}

		private static void AdjustBlockedTiles(int [][] board, int[][]goalBoard, int[][] adjustedGoalBoard)
		{
			List<(int,int)> options = new List<(int,int)> ();
			for (int row = 0; row < goalBoard.Length; row++)
			{
				for (int col = 0; col < goalBoard[row].Length; col++)
				{
					if (goalBoard[row][col] == 5) continue;

					var adjacentTiles = GetAdjacentTiles(row, col, goalBoard.Length, goalBoard[row].Length);
					var openSidesCount = adjacentTiles.Count(x => CanSwapTiles(goalBoard, row, col, x.Item1, x.Item2));
					

					//dont adjust if already in place.
					if (openSidesCount > 1 || board[row][col] == goalBoard[row][col]) 
						continue;

					var neighborsArray = GetNeighbors(board, row, col);
					var openOptions = FindOpenOptions(neighborsArray);
					var (closestOptionRow, closestOptionCol) = FindClosestInteger(board, row, col, openOptions);
					adjustedGoalBoard[row][col] = adjustedGoalBoard[closestOptionRow][closestOptionCol];
				}
			}
		}

		private static bool IsCorner(int row, int col, int numRows, int numCols)
		{
			numRows -= 1;
			numCols -= 1;

			if (row == 0 && col == numCols)
				return true;
			if (row == numRows && col == numCols)
				return true;
			if (row == numRows && col == 0)
				return true;
			if (row == 0 && col == 0)
				return true;

			return false;
		}


		private static int[][] GetNeighbors(int[][] board, int row, int col)
		{
			/*
			 * -1 - Outside Edge/Gold
			 * 
			 */
			int[][] neighbors = {
				new int[3],
				new int[3],
				new int[3]
			};

			int numRows = board.Length;
			int numCols = board[0].Length;

			int[] rowOffsets = { -1, -1, 0, 1, 1, 1, 0, -1 };
			int[] colOffsets = { 0, 1, 1, 1, 0, -1, -1, -1 };

			for (int i = 0; i < rowOffsets.Length; i++)
			{
				int newRow = row + rowOffsets[i];
				int newCol = col + colOffsets[i];

				if (newRow >= 0 && newRow < numRows && newCol >= 0 && newCol < numCols)
				{
					if (board[newRow][newCol] == 5)
					{
						neighbors[1 + rowOffsets[i]][1 + colOffsets[i]] = -1;
					}
					else
					{

						neighbors[1 + rowOffsets[i]][1 + colOffsets[i]] = board[newRow][newCol];
					}
				}
				else
				{
					neighbors[1 + rowOffsets[i]][1 + colOffsets[i]] = -1;
				}
			}

			return neighbors;
		}

		public static List<(int, int)> GetAdjacentTiles(int row, int col, int numRows, int numCols)
		{
			var adjacentTiles = new List<(int, int)>();

			int[] rowOffsets = { -1, -1, 0, 1, 1, 1, 0, -1 };
			int[] colOffsets = { 0, 1, 1, 1, 0, -1, -1, -1 };

			for (int i = 0; i < rowOffsets.Length; i++)
			{
				int newRow = row + rowOffsets[i];
				int newCol = col + colOffsets[i];

				if (newRow >= 0 && newRow < numRows && newCol >= 0 && newCol < numCols)
				{
					adjacentTiles.Add((newRow, newCol));
				}
			}

			return adjacentTiles;
		}

		private static int CountRestrictions(int[][] neighbors)
		{
			var restrictions = 0;

			for (int x = 0; x < neighbors.Length; x++)
			{
				for (int y = 0; y < neighbors[x].Length; y++)
				{
					if (neighbors[y][x] == -1)
					{
						restrictions++;
					}
				}
			}


			return restrictions;
		}

		private static List<(int row, int col)> GetPositions(int[][] board, int tile)
		{
			var positions = new List<(int, int)>();

			for (int row = 0; row < board.Length; row++)
			{
				for (int col = 0; col < board[row].Length; col++)
				{
					if (board[row][col] == tile)
					{
						positions.Add((row, col));
					}
				}
			}

			return positions;
		}

		public static bool CanSwapTiles(int[][] board, int row1, int col1, int row2, int col2)
		{
			Tile tile1 = (Tile)board[row1][col1];
			Tile tile2 = (Tile)board[row2][col2];

			if (tile1 == Tile.GOLD || tile2 == Tile.GOLD) return false;
			if (tile1 == tile2) return false;

			bool isDiagonal = Math.Abs(row1 - row2) == 1 && Math.Abs(col1 - col2) == 1;
			bool isHorizontal = row1 == row2 && Math.Abs(col1 - col2) == 1;
			bool isVertical = col1 == col2 && Math.Abs(row1 - row2) == 1;

			if (tile1 == Tile.ROPE && isDiagonal) return true;
			if (tile1 == Tile.WOOD && isVertical) return true;
			if (tile1 == Tile.CANNON && isHorizontal) return true;

			if (tile2 == Tile.ROPE && isDiagonal) return true;
			if (tile2 == Tile.WOOD && isVertical) return true;
			if (tile2 == Tile.CANNON && isHorizontal) return true;

			return false;
		}

		private static List<int> FindOpenOptions(int[][] neighbors)
		{
			var tileOptions = new List<int>();

			var offsetY = 0;
			var offsetX = 0;

			for (int x = 0; x < neighbors.Length; x++)
			{
				for (int y = 0; y < neighbors[x].Length; y++)
				{
					if (x == 1 && y == 1) continue;
					if (neighbors[y][x] != -1)
					{
						offsetY = y - 1;
						offsetX = x - 1;
					}
				}
			}

			var isDiagonal = Math.Abs(offsetY) == 1 && Math.Abs(offsetX) == 1;
			var isHorizontal = offsetY == 0;
			var isVertical = offsetX == 0;

			if (isDiagonal)
				tileOptions.Add((int)Tile.ROPE);
			if (isHorizontal)
				tileOptions.Add((int)Tile.CANNON);
			if (isVertical)
				tileOptions.Add((int)Tile.WOOD);
			
			if (tileOptions.Count == 0) 
			{
				throw new Exception();
			}

			return tileOptions;
		}





		private static (int, int) FindOpenDirection(int[][] neighbors)
		{

			var offsetY = 0;
			var offsetX = 0;

			for (int x = 0; x < neighbors.Length; x++)
			{
				for (int y = 0; y < neighbors[x].Length; y++)
				{
					if (x == 1 && y == 1) continue;
					if (neighbors[y][x] != -1)
					{
						offsetY = y - 1;
						offsetX = x - 1;
					}
				}
			}

			return (offsetX, offsetY);
			/*
			if (offsetX == -1 && offsetY == -1)
				return Direction.UPLEFT;
			if (offsetX == -1 && offsetY == 0)
				return Direction.LEFT;
			if (offsetX == -1 && offsetY == 1)
				return Direction.DOWNLEFT;

			if (offsetX == 0 && offsetY == -1)
				return Direction.UP;
			if (offsetX == 0 && offsetY == 1)
				return Direction.DOWN;

			if (offsetX == 1 && offsetY == -1)
				return Direction.UPRIGHT;
			if (offsetX == 1 && offsetY == 0)
				return Direction.RIGHT;
			if (offsetX == 1 && offsetY == 1)
				return Direction.DOWNRIGHT;

			return Direction.UNKNOWN;*/
		}
		private static int GetAdjacentWallCount(int row, int col, int numRows, int numCols)
		{
			var adjacentWallCount = 0;

			int[] rowOffsets = { -1, -1, 0, 1, 1, 1, 0, -1 };
			int[] colOffsets = { 0, 1, 1, 1, 0, -1, -1, -1 };

			for (int i = 0; i < rowOffsets.Length; i++)
			{
				int newRow = row + rowOffsets[i];
				int newCol = col + colOffsets[i];

				if (newRow < 0 || newRow >= numRows ||
				    newCol < 0 || newCol >= numCols)
				{
					adjacentWallCount++;
				}
			}

			return adjacentWallCount;
		}


	}
}
