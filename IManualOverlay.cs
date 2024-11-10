using System.Collections.Generic;

namespace ShipRight
{
	internal interface IManualOverlay
	{
		public void Run();
		public void Pause();
		public void UnPause();
		public void Stop();
		public void CreateWindow();
		public void ReCreate();
		public void StartCircle(int X, int Y, float radius);
		public void DragLine(int X, int Y, int X2, int Y2);
		public void SetText(string text);
		public void CreatePiece(List<ShapePerimeter> shapes);
		public void ClearMove();
		public bool WindowCreated { get; set; }
	}
}