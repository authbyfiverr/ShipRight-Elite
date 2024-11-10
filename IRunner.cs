namespace ShipRight
{
	internal interface IRunner
	{

		public bool IsRunning { get; set; }
		public void Run();
		public void Interrupt();
	}
}
