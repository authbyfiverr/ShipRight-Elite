using LockedBitmapUtil;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace ShipRight
{
	internal interface IBoardReader
	{
		public Point BoardOffset { get; set; }
		public bool TryGetBoard(out int[][] gameBoard);
		public bool TryGetPieces(out List<Piece> currentPieces);
		public bool TryGetFlag(out int flagPos);
		public Point GetGridCenterPoint(int x, int y);
		public Point GetGridPoint(int x, int y);
		public bool GetAnchorPoint(IntPtr Handle);
		public bool InPuzzle(IntPtr handle, out PuzzleStatus windowstatus);
		public bool InPuzzle(LockedBitmap screenShot, out PuzzleStatus windowstatus);
		public string PrintBoard(int[][] board);
		public Piece InitPiece(Pieces piece);
		public List<Piece> AllPieces { get; set; }


	}
}