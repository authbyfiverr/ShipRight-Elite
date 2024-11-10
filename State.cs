using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

namespace ShipRight
{
	internal class State
	{
		public int[][] Board { get; }
		public int distanceToGoal { get; set; }
		public State Previous { get; }
		public double Cost { get; }
		public double FScore { get; }
		public int Depth { get; }
		public int DepthLimit { get; set; }
		public int MovedRow { get; set; }
		public int MovedCol { get; set; }
		

		public State(int[][] board, State previous, int movedRow, int movedCol, double cost, double fScore, int depth, int depthLimit)
		{
			Board = board;
			Previous = previous;
			Cost = cost;
			FScore = fScore;
			Depth = depth;
			DepthLimit = depthLimit;
			MovedRow = movedRow;
			MovedCol = movedCol;
		}
	}

	class StateComparer : IComparer<State>
	{
		public int Compare(State x, State y)
		{
			int result = x.FScore.CompareTo(y.FScore);
			return result != 0 ? result : x.GetHashCode().CompareTo(y.GetHashCode());
		}
	}


}
