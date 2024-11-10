using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipRight
{
	internal class SolverNoMoves
	{
		double bestScore = 0;
		int[][] bestBoard;
		Dictionary<Tile, int> bestRemainingTiles;
		private List<Piece> usedPieces;

		public void InitSolve(int[][] board, List<Piece> currentPieces, Dictionary<Tile, int> currentTiles)
		{

			Solve(board, new List<Piece>(currentPieces),0, currentTiles);


		}

		public List<Piece> GetUsedPieces()
		{
			return usedPieces.ToList();

		}


		private void Solve(int[][] board, List<Piece> pieces, double score, Dictionary<Tile, int> remainingTiles)
		{
			
			if (score > bestScore)
			{
				bestScore = score;
				bestBoard = board.DeepClone();
				bestRemainingTiles = new Dictionary<Tile, int>(remainingTiles);
				usedPieces = DeepCopyPieces(pieces);
			}
			
			
			foreach (Piece piece in pieces)
			{
				if (piece.Used) continue; 

				foreach (var placement in GetValidPlacements(board, piece, remainingTiles))
				{
					Dictionary<Tile, int> updatedRemainingTiles = PlacePiece(board, placement, remainingTiles);
					piece.Used = true;
					var newScore = CalculateScore(board, pieces, remainingTiles);
					Solve(board, pieces, newScore, updatedRemainingTiles);
					RemovePiece(board, placement, remainingTiles);
					piece.Used = false;
				}
			}
		}

		List<Placement> GetValidPlacements(int[][] board, Piece piece, Dictionary<Tile, int> remainingTiles)
		{
			var placements = new List<Placement>();
			for (int row = 0; row < board.Length; row++)
			{
				for (int col = 0; col < board[row].Length; col++)
				{
					if (CanPlacePiece(board, piece, row, col, remainingTiles))
					{
						placements.Add(new Placement { Piece = piece, Row = row, Column = col });
					}
				}
			}
			return placements;
		}

		private bool CanPlacePiece(int[][] board, Piece piece, int row, int col, Dictionary<Tile, int> remainingTiles)
		{
			int[][] shape = piece.Shape;
			int shapeRows = shape.Length;
			int shapeCols = shape[0].Length;

			if (row + shapeRows > board.Length || col + shapeCols > board[0].Length)
			{
				return false;
			}

			Dictionary<Tile, int> tempRemainingTiles = new Dictionary<Tile, int>(remainingTiles);

			for (int r = 0; r < shapeRows; r++)
			{
				for (int c = 0; c < shapeCols; c++)
				{
					if (shape[r][c] != 0)
					{
						Tile tileType = (Tile)shape[r][c];
						Tile boardTileType = (Tile)board[row + r][col + c];
						if (tileType != boardTileType && boardTileType != Tile.GOLD)
						{
							return false;
						}

						if (tempRemainingTiles[tileType] > 0)
						{
							tempRemainingTiles[tileType]--;
						}
						else if (boardTileType != Tile.GOLD)
						{
							return false;
						}
					}
				}
			}

			return true;
		}

		private List<Piece> DeepCopyPieces(List<Piece> pieces)
		{
			return pieces.Select(piece => piece.Clone()).ToList();
		}

		private Dictionary<Tile, int> PlacePiece(int[][] board, Placement placement, Dictionary<Tile, int> remainingTiles)
		{
			Dictionary<Tile, int> updatedRemainingTiles = new Dictionary<Tile, int>(remainingTiles);
			int[][] shape = placement.Piece.Shape;
			int shapeRows = shape.Length;
			int shapeCols = shape[0].Length;

			for (int r = 0; r < shapeRows; r++)
			{
				for (int c = 0; c < shapeCols; c++)
				{
					if (shape[r][c] != 0)
					{
						Tile tileType = (Tile)shape[r][c];
						Tile boardTileType = (Tile)board[placement.Row + r][placement.Column + c];
						if (tileType == boardTileType || boardTileType == Tile.GOLD)
						{
							if (updatedRemainingTiles[tileType] > 0)
							{
								updatedRemainingTiles[tileType]--;
							}
							placement.Piece.BoardPos[placement.Row + r][placement.Column + c] = new Point(c, r) == placement.Piece.OriginPoint ? 99 : (int)boardTileType;
							board[placement.Row + r][placement.Column + c] = 0; // Mark the tile as used
						}
					}
				}
			}

			return updatedRemainingTiles;
		}

		private void RemovePiece(int[][] board, Placement placement, Dictionary<Tile, int> remainingTiles)
		{
			int[][] shape = placement.Piece.Shape;
			int shapeRows = shape.Length;
			int shapeCols = shape[0].Length;

			for (int r = 0; r < shapeRows; r++)
			{
				for (int c = 0; c < shapeCols; c++)
				{
					if (shape[r][c] != 0)
					{
						var tileType = (Tile)shape[r][c];
						remainingTiles[tileType]++;
						board[placement.Row + r][placement.Column + c] = shape[r][c];
						placement.Piece.BoardPos[placement.Row + r][placement.Column + c] = 0;

					}
				}
			}
		}



		private double CalculateScore(int[][] board, List<Piece> pieces, Dictionary<Tile, int> remainingTiles)
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

			score = pieceScores.Average(); ;

			return score;
		}






	}
}
