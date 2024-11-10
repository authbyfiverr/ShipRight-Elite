using LockedBitmapUtil;
using System.Drawing;
using System.Threading;

namespace ShipRight
{
	internal interface IAction
	{
		public void StartStation();
		public void ClickPlayButton();
		public void ExitPuzzle();
		public void PlayAgain();
		public bool CheckForPlayAgain();


	}
}