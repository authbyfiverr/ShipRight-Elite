using System;
using System.Diagnostics;
using System.Drawing;

namespace ShipRight
{
	internal class Configuration : IConfiguration
	{
		public int AuthValue { get; set; } = 12;
		public bool Automatic { get; set; }
		public IntPtr PpWindow { get; set; }
		public Process PpProcess { get; set; }
		public int MouseSpeed { get; set; } = 8;

		public Point BoardOffset { get; set; }
		public bool RejectBoards { get; set; }
		public decimal RejectScore { get; set; }
		public bool ForceScan { get; set; }
		public int FinishChain { get; set; }

	}
}
