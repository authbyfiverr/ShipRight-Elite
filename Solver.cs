using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ShipRight
{
	internal class Solver
	{
		private double bestScore = 0;
		private double bestRefinedScore = 0;
		private double bestFinalScore = 0;

		private List<Piece> _allPieces;

		private HashSet<string> _savedBoards = new HashSet<string>();
		private List<Solution> solutions;
		private List<Solution> refinedSolutions = new List<Solution>();
		private List<Solution> finalSolutions = new List<Solution>();

		private readonly object scoreLock = new object();
		private string _output;


		public double GetBestScore()
		{
			return bestScore;
		}

		public string GetOutput()
		{
			return _output;
		}

		public string GetUsedPieces()
		{
			return _output;
		}

		public Solution InitSolve(int[][] board, List<Piece> pieces, double score, Dictionary<Tile, int> remainingTiles,
			bool[][] usedTilesBoard, List<Piece> allPieces)
        {
            _allPieces = new List<Piece>(allPieces);

            solutions = new List<Solution>();
			Solve(board, pieces, score, remainingTiles, usedTilesBoard);
			Debug.WriteLine($"Total solutions: {solutions.Count}\n" +
			                $"Best Score: {bestScore}");
			
			MainForm.SetLabel(MainForm.Labels.SolveScore, $"{bestScore}");

			var maxWildsLeft = solutions.Max(x => x.WildsLeft);
			
			foreach (var solution in solutions)
			{
				BonusSolve(solution.Board.DeepClone(), DeepCopyPieces(solution.Pieces), solution.Score, DeepCopyRemainingTiles(solution.RemainingTiles), solution.UsedTilesBoard.DeepClone());
			}

			foreach (var solution in refinedSolutions)
			{
				var threeCount = solution.Pieces.Count(x => x.Used && x.Size == 3);
				var fourCount = solution.Pieces.Count(x => x.Used && x.Size == 4);
				var fiveCount = solution.Pieces.Count(x => x.Used && x.Size == 5);
				var counts = new int[] { threeCount, fourCount, fiveCount };

				var optionPieces = solution.Pieces.Concat(_allPieces.Where(x => solution.Pieces.All(o => o.Name != x.Name))).ToList();
				
				FinalSolve(solution.Board.DeepClone(), DeepCopyPieces(optionPieces), solution.Score, DeepCopyRemainingTiles(solution.RemainingTiles), solution.UsedTilesBoard.DeepClone(), counts.ToArray());
			}

            List<Solution> orderedSolutions;
			orderedSolutions = finalSolutions.OrderByDescending(s => s.Score)
													.ThenBy(s => s.RemainingTiles.Values.Max() - s.RemainingTiles.Values.Min())
													.ToList();

            var usedPieces = orderedSolutions[0].Pieces
				.Where(x => (x.Used) && pieces.Any(o => o.Name == x.Name))
				.OrderBy(x => x.Size)
				.ToList();
			var partialPieces = orderedSolutions[0].Pieces
				.Where(x => (x.PartialUsed))
				.OrderBy(x => x.Size)
				.ToList();
			var setupPieces = orderedSolutions[0].Pieces
				.Where(x => (x.IsBonus) && pieces.All(o => o.Name != x.Name))
				.OrderBy(x => x.Size)
				.ToList();

			_output = $"" +
			          $"Best Score: \t{orderedSolutions[0].Score}\n" +
			          $"Pieces Used: \t{string.Join(", ", usedPieces.Select(x => x.Name))}\n" +
			          $"Pieces Partial: \t{string.Join(", ", partialPieces.Select(x => x.Name))}\n" +
			          $"Pieces Setup: \t{string.Join(", ", setupPieces.Select(x => x.Name))}\n";

			MainForm.SetLabel(MainForm.Labels.BonusScore, $"{orderedSolutions[0].Score}");

			Debug.WriteLine( _output );
			foreach (KeyValuePair<Tile, int> kvp in orderedSolutions[0].RemainingTiles)
			{
				Debug.WriteLine("{0}: {1}", kvp.Key, kvp.Value);
			}

			FillZeros(orderedSolutions[0].Board, orderedSolutions[0].RemainingTiles);

			PrintJaggedArray(orderedSolutions[0].Board);
			return orderedSolutions[0];
		}

		private void FillZeros(int[][] board, Dictionary<Tile, int> remainingTiles)
		{
			for (int i = 0; i < board.Length; i++)
			{
				for (int j = 0; j < board[i].Length; j++)
				{
					if (board[i][j] == 0)
					{
						foreach (KeyValuePair<Tile, int> kvp in remainingTiles.ToList())
						{
							if (kvp.Value == 0) continue;
							if (kvp.Key == Tile.SAIL || kvp.Key == Tile.ROPE || kvp.Key == Tile.CANNON || kvp.Key == Tile.WOOD)
							{
								board[i][j] = (int)kvp.Key;
								remainingTiles[kvp.Key]--; // Update the value in the dictionary
								break;
							}
						}
					}
				}
			}
		}

		public static void PrintJaggedArray(int[][] array)
		{
			for (int i = 0; i < array.Length; i++)
			{
				Debug.WriteLine("");
				for (int j = 0; j < array[i].Length; j++)
				{
					Debug.Write($"{(Tile)array[i][j]}\t");
				}
			}
		}


		private void Solve(int[][] board, List<Piece> pieces, double score, Dictionary<Tile, int> remainingTiles, bool[][] usedTilesBoard)
		{
			
			if (score >= bestScore)
			{
				if (score > bestScore)
					solutions.Clear();

				string newBoardHash = BoardToString(board);
				if (!_savedBoards.Contains(newBoardHash))
				{
					solutions.Add(new Solution(board, pieces, score, remainingTiles, usedTilesBoard));
					_savedBoards.Add(newBoardHash);
					bestScore = score;
				}
			}

            if (solutions.Count > 200)
                return;

            foreach (var piece in pieces)
			{
				if (piece.Used) continue;

				foreach (var placement in GetValidPlacements(board, piece, remainingTiles, usedTilesBoard))
				{
					var previousUsedTilesState = usedTilesBoard.DeepClone();
					var (updatedRemainingTiles, previousBoardState) = PlacePiece(board, placement, remainingTiles, usedTilesBoard);
					piece.Used = true;
					var newScore = CalculateScore(board, pieces, updatedRemainingTiles);
					
					Solve(board, pieces, newScore, updatedRemainingTiles, usedTilesBoard);
					RemovePiece(board, placement, remainingTiles, usedTilesBoard, previousBoardState, previousUsedTilesState);
					piece.Used = false;
				}
			}
		}

		private void BonusSolve(int[][] board, List<Piece> pieces, double score, Dictionary<Tile, int> remainingTiles, bool[][] usedTilesBoard)
		{
			if (score >= bestRefinedScore)
			{
				
				if (score > bestRefinedScore)
					refinedSolutions.Clear();

				bestRefinedScore = score;
				refinedSolutions.Add(new Solution(board.DeepClone(), DeepCopyPieces(pieces), score, DeepCopyRemainingTiles(remainingTiles), DeepCopyUsedTilesBoard(usedTilesBoard)));
			}
			if (refinedSolutions.Count > 200)
				return;

			foreach (var piece in pieces)
			{
				if (piece.Used || piece.PartialUsed) continue;

				foreach (var placement in GetValidPlacements(board, piece, remainingTiles, usedTilesBoard, true))
				{
					var previousUsedTilesState = usedTilesBoard.DeepClone();
					var (updatedRemainingTiles, previousBoardState) = PlacePiece(board, placement, remainingTiles, usedTilesBoard);
					piece.PartialUsed = true;
					var newScore = CalculateScore(board, pieces, updatedRemainingTiles);

					BonusSolve(board, pieces, newScore, updatedRemainingTiles, usedTilesBoard);
					RemovePiece(board, placement, remainingTiles, usedTilesBoard, previousBoardState, previousUsedTilesState);
					piece.PartialUsed = false;
				}
			}
		}

		private void FinalSolve(int[][] board, List<Piece> pieces, double score, Dictionary<Tile, int> remainingTiles,
			bool[][] usedTilesBoard, int[] counts)
		{
			if (score >= bestFinalScore)
			{
				if (score > bestFinalScore)
					finalSolutions.Clear();

				bestFinalScore = score;
				finalSolutions.Add(new Solution(board.DeepClone(), DeepCopyPieces(pieces), score, DeepCopyRemainingTiles(remainingTiles), DeepCopyUsedTilesBoard(usedTilesBoard)));
			}

			if (finalSolutions.Count > 200)
				return;

			foreach (var piece in pieces)
			{
				if (piece.Used || piece.PartialUsed || piece.IsBonus) continue;
				switch (piece.Size)
				{
					case 3 when counts[0] == 0:
					case 4 when counts[1] == 0:
					case 5 when counts[2] == 0:
						continue;
				}

				foreach (var placement in GetValidPlacements(board, piece, remainingTiles, usedTilesBoard))
				{
					var previousUsedTilesState = usedTilesBoard.DeepClone();
					var (updatedRemainingTiles, previousBoardState) = PlacePiece(board, placement, remainingTiles, usedTilesBoard);
					piece.IsBonus = true;
					switch (piece.Size)
					{
						case 3: counts[0]--; break;
						case 4: counts[1]--; break;
						case 5: counts[2]--; break;
					}

					var newScore = CalculateScore(board, pieces, updatedRemainingTiles, true);

					FinalSolve(board, pieces, newScore, updatedRemainingTiles, usedTilesBoard, counts.ToArray());
					RemovePiece(board, placement, remainingTiles, usedTilesBoard, previousBoardState, previousUsedTilesState);
					piece.IsBonus = false;
					switch (piece.Size)
					{
						case 3: counts[0]++; break;
						case 4: counts[1]++; break;
						case 5: counts[2]++; break;
					}
				}
			}
		}


		private List<Placement> GetValidPlacements(int[][] board, Piece piece, Dictionary<Tile, int> remainingTiles,
			bool[][] placedPieces, bool allowOverlap = false)
		{
			var placements = new List<Placement>();
			for (var row = 0; row < board.Length; row++)
			{
				for (var col = 0; col < board[row].Length; col++)
				{
					if (CanPlacePiece(board, piece, row, col, remainingTiles, placedPieces, allowOverlap))
					{
						placements.Add(new Placement { Piece = piece, Row = row, Column = col });
					}
				}
			}
			return placements;
		}

		private bool CanPlacePiece(int[][] board, Piece piece, int row, int col, Dictionary<Tile, int> remainingTiles,
			bool[][] usedTilesBoard, bool allowOverlap = false)
		{
			bool overlapping = false;
			var overlap = 0;
			var shape = piece.Shape;
			var shapeRows = shape.Length;
			var shapeCols = shape[0].Length;

			if (row + shapeRows > board.Length || col + shapeCols > board[0].Length)
			{
				return false;
			}

			var tempRemainingTiles = new Dictionary<Tile, int>(remainingTiles);

			for (var r = 0; r < shapeRows; r++)
			{
				for (var c = 0; c < shapeCols; c++)
				{
					if (shape[r][c] != 0)
					{
						// Check if a piece is already placed at this location
						if (usedTilesBoard[row + r][col + c])
						{
							overlap++;
							overlapping = true;
							if (!allowOverlap || overlap > 1)
							{
								//Debug.WriteLine($"Returning false because too many overlaps for piece {piece.Name} at {row} {col}.");
								return false;
							}
						}

						if (overlapping)
						{
							overlapping = false;
							continue;
						}

						var tileType = (Tile)shape[r][c];
						var boardTileType = (Tile)board[row + r][col + c];

						if (boardTileType == Tile.GOLD)
							tempRemainingTiles[boardTileType]--;
						else if (tempRemainingTiles[tileType] > 0)
						{
							tempRemainingTiles[tileType]--;
						}
						else
						{
							return false;
						}
					}
				}
			}
			
			return true;
		}

		private (Dictionary<Tile, int>, int[][]) PlacePiece(int[][] board, Placement placement, Dictionary<Tile, int> remainingTiles, bool[][] usedTilesBoard)
		{
			var previousBoardState = board.Select(row => row.ToArray()).ToArray();
			bool overlap = false;

			var updatedRemainingTiles = new Dictionary<Tile, int>(remainingTiles);
			var shape = placement.Piece.Shape;
			var shapeRows = shape.Length;
			var shapeCols = shape[0].Length;

			for (var r = 0; r < shapeRows; r++)
			{
				for (var c = 0; c < shapeCols; c++)
				{
					if (shape[r][c] != 0)
					{
						if (usedTilesBoard[placement.Row + r][placement.Column + c])
						{
							//Ignoring overlap
							overlap = true;
							continue;
						}

						var tileType = (Tile)shape[r][c];
						var boardTileType = (Tile)board[placement.Row + r][placement.Column + c];
						if (boardTileType == Tile.GOLD)
						{
							updatedRemainingTiles[boardTileType]--;
							placement.Piece.BoardPos[placement.Row + r][placement.Column + c] = new Point(c,r) == placement.Piece.OriginPoint ? 99 : (int)boardTileType;
						}
						else
						{
							updatedRemainingTiles[tileType]--;
							board[placement.Row + r][placement.Column + c] = (int)tileType;
							placement.Piece.BoardPos[placement.Row + r][placement.Column + c] = new Point(c, r) == placement.Piece.OriginPoint ? 99 : (int)tileType;
						}
						usedTilesBoard[placement.Row + r][placement.Column + c] = true; // Mark the tile as used

					}
				}
			}

			if (overlap)
			{
				placement.Piece.PartialUsed = true;
			}
			else
			{
				placement.Piece.Used = true; // Mark the piece as used
			}
			
			return (updatedRemainingTiles, previousBoardState);
		}

		private void RemovePiece(int[][] board, Placement placement, Dictionary<Tile,int> remainingTiles, bool[][] placedPieces, int[][] previousBoardState, bool[][] previousPlacedPieces)
		{
			var updatedRemainingTiles = new Dictionary<Tile, int>(remainingTiles);
			var shape = placement.Piece.Shape;
			var shapeRows = shape.Length;
			var shapeCols = shape[0].Length;

			for (var r = 0; r < shapeRows; r++)
			{
				for (var c = 0; c < shapeCols; c++)
				{
					if (shape[r][c] != 0)
					{
						//If we didn't use this tile location before
						if (!previousPlacedPieces[placement.Row + r][placement.Column + c])
						{
							var tileType = (Tile)shape[r][c];
							var boardTileType = (Tile)previousBoardState[placement.Row + r][placement.Column + c];

							if (boardTileType == Tile.GOLD)
							{
								updatedRemainingTiles[boardTileType]++;
							}
							else
							{
								updatedRemainingTiles[tileType]++;
							}
							
							placement.Piece.BoardPos[placement.Row + r][placement.Column + c] = 0;

							placedPieces[placement.Row + r][placement.Column + c] = previousPlacedPieces[placement.Row + r][placement.Column + c];
							board[placement.Row + r][placement.Column + c] = previousBoardState[placement.Row + r][placement.Column + c];
						}

					}
				}
			}
			placement.Piece.Used = false; // Mark the piece as unused
		}


		private double CalculateScore(int[][] board, List<Piece> pieces, Dictionary<Tile, int> remainingTiles, bool bonus = false)
		{
			double score = 0;
			int chainPosition = 1;
			var pieceScores = new List<int>(); // stores the piece scores


			var orderedPieces = new List<Piece>(pieces).Where(x => x.Used).OrderBy(x => x.Size).ToList();

			// Calculate score for placed pieces based on their position in the chain
			foreach (var piece in orderedPieces)
			{
				if (piece.Used)
				{
					var pieceScore = (piece.Size - 2) * chainPosition;
					pieceScores.Add(pieceScore);
					score += pieceScore;
					chainPosition++;
				}
			}

			foreach (var piece in pieces)
			{
				if (piece.PartialUsed)
				{
					Debug.WriteLine($"Partial Piece used {piece.Name}");
					var pieceScore = (piece.Size - 2) * chainPosition;
					pieceScores.Add(pieceScore);
					score += pieceScore-1;
					chainPosition++;
				}
			}

			foreach (var piece in pieces)
			{
				if (piece.IsBonus)
				{
					//Debug.WriteLine($"Partial Piece used {piece.Name}");
					var pieceScore = (piece.Size - 2) * chainPosition;
					pieceScores.Add(pieceScore);
					score += pieceScore - 1;
					chainPosition++;
				}
			}

			score = pieceScores.Average(); ; 

			return score;
		}

		
		private string BoardToString(int[][] board)
		{
			StringBuilder sb = new StringBuilder();
			for (int row = 0; row < board.Length; row++)
			{
				for (int col = 0; col < board[row].Length; col++)
				{
					sb.Append(board[row][col]);
					sb.Append(",");
				}
				sb.Append(";");
			}
			return sb.ToString();
		}

		
		private List<Piece> DeepCopyPieces(List<Piece> pieces)
		{
			return pieces.Select(piece => piece.Clone()).ToList();
		}

		private Dictionary<Tile, int> DeepCopyRemainingTiles(Dictionary<Tile, int> remainingTiles)
		{
			return new Dictionary<Tile, int>(remainingTiles);
		}

		private bool[][] DeepCopyUsedTilesBoard(bool[][] usedTilesBoard)
		{
			return usedTilesBoard.Select(row => (bool[])row.Clone()).ToArray();
		}

	}

	class Placement
	{
		public Piece Piece;
		public int Row;
		public int Column;
	}



	class Solution
	{
		public int[][] Board { get; set; }
		public List<Piece> Pieces { get; set; }
		public double Score { get; set; }
		public Dictionary<Tile, int> RemainingTiles { get; set; }
		public bool[][] UsedTilesBoard { get; set; }
		public string UsedPieces { get; set; }
		public int WildsLeft { get; set; }

		public Solution(int[][] board, List<Piece> pieces, double score, Dictionary<Tile, int> remainingTiles,
			bool[][] usedTilesBoard)
		{

			Board = (int[][])board.Clone();
				for (var i = 0; i < board.Length; i++)
				{
					Board[i] = (int[])board[i].Clone();
				}
			Pieces = pieces.Select(piece => piece.Clone()).ToList();
			Score = score;
			RemainingTiles = new Dictionary<Tile, int>(remainingTiles);
			UsedTilesBoard = (bool[][])usedTilesBoard.Clone();
				for (var i = 0; i < usedTilesBoard.Length; i++)
				{
					UsedTilesBoard[i] = (bool[])usedTilesBoard[i].Clone();
				}

				WildsLeft = remainingTiles[Tile.GOLD];
		}

	}

}
