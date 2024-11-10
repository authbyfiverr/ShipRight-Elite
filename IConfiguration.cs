using System;
using System.Diagnostics;
using System.Drawing;

namespace ShipRight
{
	internal interface IConfiguration
	{
		public int AuthValue { get; set; }
		public bool Automatic { get; set; }
		public IntPtr PpWindow { get; set; }
		public Process PpProcess { get; set; }
		public int MouseSpeed { get; set; }
		public Point BoardOffset { get; set; }

		public bool RejectBoards { get; set; }
		public decimal RejectScore { get; set; }
		public bool ForceScan { get; set; }
		public int FinishChain { get; set; }

	}
}
