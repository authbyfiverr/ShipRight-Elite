using System.Collections.Generic;
using System.Threading;

namespace ShipRight
{
	internal interface IPuzzler
	{
		public void Puzzle(CancellationToken cancellationToken, int flagPos, int[][] gameBoard, List<Piece> currentPieces);
		public void Reset();
	}
}