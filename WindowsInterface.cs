using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ShipRight
{
	internal static class WindowsInterface
	{
		public static IEnumerable<(string clientName, IntPtr clientHandle)> GetOpenPuzzlePiratesClients()
		{
			foreach (var process in Process.GetProcesses())
				if (process.MainWindowTitle.Contains("Puzzle Pirates -") &&
					process.MainWindowTitle.Contains(" on the ") &&
					process.MainWindowTitle.Contains("ocean"))
					yield return (process.MainWindowTitle.ToShortClientName(), process.MainWindowHandle);
		}

		private static string ToShortClientName(this string clientName)
		{
			var segmentedName = clientName.Split('-')[1].Split("on the");

			return $"{segmentedName[0].Trim()} - {segmentedName[1].Trim().Split(' ')[0]}";
		}

		public static Point GetClientPosition(IntPtr clientHandle)
		{
			User32.RECT windowRect = new User32.RECT();
			User32.GetWindowRect(clientHandle, ref windowRect);

			return new Point(windowRect.left, windowRect.top);
		}

		private class User32
		{
			[StructLayout(LayoutKind.Sequential)]
			public struct RECT
			{
				public int left;
				public int top;
				public int right;
				public int bottom;
			}
			[DllImport("user32.dll", SetLastError = true)]
			public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
		}
	}
}
