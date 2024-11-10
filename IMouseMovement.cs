using System.Drawing;
using System.Threading.Tasks;

namespace ShipRight
{
	internal interface IMouseMovement
	{
		public bool HumanMoveMouseMovement(System.Drawing.Point endLocation, int speed = 5, bool hmm = true);
		public void LeftClick();
		public void DoubleLeftClick();
		public void RightClick();
		public void PressKey(byte keycode);
		public void PressKeyInstant(byte keycode);
		public void ClearAverages();

		public void PauseGame();
		public void UnPauseGame();

		public void LeftDown();
		public bool IsLeftPressed();
		public void LeftUp();
	}
}
