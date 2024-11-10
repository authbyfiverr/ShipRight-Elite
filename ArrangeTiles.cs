using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ShipRight.Heuristic;
using static ShipRight.Extensions;

namespace ShipRight
{
	internal static class ArrangeTiles
	{
		private const int INITIAL_DEPTH_LIMIT = 25;

		public static List<Swap> FindTileSwaps(int[][] startBoard, int[][] goalBoard, CancellationToken cancellationToken)
		{
			var openSet = new SortedSet<State>(new StateComparer());
			var closedSet = new HashSet<string>();

			var startState = new State(startBoard, null,0,0, 0, GetHeuristic(startBoard, goalBoard), 0, INITIAL_DEPTH_LIMIT);
			openSet.Add(startState);
			var bestState = startState;

			while (openSet.Count > 0 && !cancellationToken.IsCancellationRequested)
			{
				var currentState = openSet.First();
				openSet.Remove(currentState);

				if (BoardsAreEqual(currentState.Board, goalBoard))
				{
					Debug.WriteLine($"Closed Sets: {closedSet.Count}");
					return GetSwapsFromPath(currentState);
				}

				if (currentState.Depth >= currentState.DepthLimit)
				{
					continue;
				}

				if (currentState.FScore < bestState.FScore)
				{
					bestState = currentState;
				}

				closedSet.Add(BoardToString(currentState.Board));

				var numRows = currentState.Board.Length;
				var numCols = currentState.Board[0].Length;

				for (int row = 0; row < numRows; row++)
				{
					for (int col = 0; col < numCols; col++)
					{
						foreach (var (adjRow, adjCol) in GetAdjacentTiles(row, col, numRows, numCols))
						{
							if (!CanSwapTiles(currentState.Board, row, col, adjRow, adjCol))
							{
								continue;
							}

							var newBoard = SwapTiles(currentState.Board, row, col, adjRow, adjCol);
							var newBoardStr = BoardToString(newBoard);

							if (closedSet.Contains(newBoardStr))
							{
								continue;
							}

							double newCost = currentState.Cost + .5;

							if (row != currentState.MovedRow || col != currentState.MovedCol)
							{
								newCost += 2;
							}

							var newHeuristic = GetHeuristic(newBoard, goalBoard);
							var newDepthLimit = currentState.DepthLimit;

							if (newHeuristic <= bestState.FScore * 1.1)
							{
								newDepthLimit += 5;
							}

							if (newDepthLimit > 60)
								newDepthLimit = 60;


							var newState = new State(newBoard, currentState,adjRow,adjCol, newCost, newCost + newHeuristic, currentState.Depth + 1, newDepthLimit);
							openSet.RemoveWhere(s => BoardsAreEqual(s.Board, newBoard) && s.Cost >= newCost);
							if (openSet.Count > 2000)
							{
								openSet.RemoveWhere(s => s.FScore * 0.95 > newHeuristic + newCost);
							}
							if (!openSet.Any(s => BoardsAreEqual(s.Board, newBoard)))
							{
								openSet.Add(newState);
							}
						}
					}
				}
			}

			Debug.WriteLine($"Closed Sets: {closedSet.Count}");

			return null;
		}


		public static List<Swap> FindTileSwaps2(int[][] startBoard, int[][] goalBoard, CancellationToken cancellationToken)
		{
			var openSet = new SortedSet<State>(new StateComparer());
			var closedSet = new HashSet<string>();

			var startState = new State(startBoard, null, -1, -1, 0, GetHeuristic(startBoard, goalBoard), 0, INITIAL_DEPTH_LIMIT);
			openSet.Add(startState);
			var bestState = startState;

			while (openSet.Count > 0 && !cancellationToken.IsCancellationRequested)
			{
				var currentState = openSet.First();
				openSet.Remove(currentState);

				if (currentState.FScore < bestState.FScore)
				{
					bestState = currentState;
					//Debug.WriteLine($"Current Limit: \t{bestState.DepthLimit}\nCurrent Score: \t{bestState.FScore,0:00.00}\nCurrent Cost: \t{bestState.Cost}\n");
				}

				if (BoardsAreEqual(currentState.Board, goalBoard))
				{
					Debug.WriteLine($"Closed Sets: {closedSet.Count}");
					return GetSwapsFromPath(currentState);
				}

				if (currentState.Depth >= currentState.DepthLimit)
				{
					continue;
				}

				closedSet.Add(BoardToString(currentState.Board));

				var numRows = currentState.Board.Length;
				var numCols = currentState.Board[0].Length;

				for (int row = 0; row < numRows; row++)
				{
					for (int col = 0; col < numCols; col++)
					{
						foreach (var (adjRow, adjCol) in GetAdjacentTiles(row, col, numRows, numCols))
						{
							if (!CanSwapTiles(currentState.Board, row, col, adjRow, adjCol))
							{
								continue;
							}

							var newBoard = SwapTiles(currentState.Board, row, col, adjRow, adjCol);
							var newBoardStr = BoardToString(newBoard);

							if (closedSet.Contains(newBoardStr))
							{
								continue;
							}

							double newCost = currentState.Cost + 1;

							if (row != currentState.MovedRow || col != currentState.MovedCol)
							{
								newCost += 1;
							}

							var newHeuristic = GetHeuristic(newBoard, goalBoard);
							var newDepthLimit = currentState.DepthLimit;

							if (newHeuristic <= bestState.FScore * 1.1)
							{
								newDepthLimit += 5;
								if (newDepthLimit > 50)
									newDepthLimit = 50;
							}
							

							var newState = new State(newBoard, currentState, adjRow, adjCol, newCost, newCost + newHeuristic, currentState.Depth + 1, newDepthLimit);
							openSet.RemoveWhere(s => BoardsAreEqual(s.Board, newBoard) && s.Cost >= newCost);

							if (openSet.Count > 2000)
							{
								openSet.RemoveWhere(s => s.FScore * 0.95 > newHeuristic + newCost);
							}

							if (!openSet.Any(s => BoardsAreEqual(s.Board, newBoard)))
							{
								openSet.Add(newState);
							}
						}
					}
				}
			}

			Debug.WriteLine($"Closed Sets: {closedSet.Count}");

			return null;
		}


		private static string BoardToString(int[][] board)
		{
			return string.Join(";", board.Select(row => string.Join(",", row)));
		}

		private static bool BoardsAreEqual(int[][] board1, int[][] board2)
		{
			for (int row = 0; row < board1.Length; row++)
			{
				for (int col = 0; col < board1[row].Length; col++)
				{
					if (board1[row][col] != board2[row][col])
					{
						return false;
					}
				}
			}
			return true;
		}


		private static int[][] SwapTiles(int[][] board, int row1, int col1, int row2, int col2)
		{
			var newBoard = board.DeepClone();
			(newBoard[row1][col1], newBoard[row2][col2]) = (newBoard[row2][col2], newBoard[row1][col1]);

			return newBoard;
		}

		private static List<Swap> GetSwapsFromPath(State finalState)
		{
			var swaps = new List<Swap>();
			var currentState = finalState;
			while (currentState.Previous != null)
			{
				var (row1, col1, row2, col2) = FindSwappedTiles(currentState.Previous.Board, currentState.Board);
				var tile1Type = (Tile)currentState.Board[row1][col1];
				var tile2Type = (Tile)currentState.Board[row2][col2];

				// Check the distanceToGoal property to determine the order of the tiles and boards
				if (currentState.Previous.distanceToGoal < currentState.distanceToGoal)
				{
					swaps.Add(new Swap(new Point(col1, row1), new Point(col2, row2),
						tile1Type, tile2Type,
						currentState.Previous.Board.DeepClone(), currentState.Board.DeepClone()));
				}
				else
				{
					swaps.Add(new Swap(new Point(col2, row2), new Point(col1, row1),
						tile2Type, tile1Type,
						currentState.Board.DeepClone(), currentState.Previous.Board.DeepClone()));
				}

				currentState = currentState.Previous;
			}

			swaps.Reverse();
			return swaps;
		}

		private static (int row1, int col1, int row2, int col2) FindSwappedTiles(int[][] board1, int[][] board2)
		{
			int numRows = board1.Length;
			int numCols = board1[0].Length;

			int? row1 = null, col1 = null, row2 = null, col2 = null;
			for (int row = 0; row < numRows; row++)
			{
				for (int col = 0; col < numCols; col++)
				{
					if (board1[row][col] != board2[row][col])
					{
						if (row1 == null)
						{
							row1 = row;
							col1 = col;
						}
						else
						{
							row2 = row;
							col2 = col;
							return (row1.Value, col1.Value, row2.Value, col2.Value);
						}
					}
				}
			}

			throw new ArgumentException("Tiles are not swappable.");
		}
	}
}
