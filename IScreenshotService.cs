using LockedBitmapUtil;
using System;
using System.Drawing;

namespace ShipRight
{
	internal interface IScreenshotService
	{
		public LockedBitmap CaptureScreen(IntPtr clientHandle);
	}
}
