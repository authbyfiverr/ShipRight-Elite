using LockedBitmapUtil;
using LockedBitmapUtil.Extensions;
using ShipRight.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ShipRight.Extensions;
using static ShipRight.MainForm;
using static System.Windows.Forms.DataFormats;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace ShipRight
{
	internal class Action : IAction
	{
		private readonly IMouseMovement _mouseMovement;
		private readonly IConfiguration _configuration;
		private readonly IScreenshotService _screenshotService;
		private readonly IColorComparator _colorComparator;
		private readonly Random _random = new Random();
		private Stopwatch _sleepStopwatch = new Stopwatch();

		public Action(IMouseMovement mouseMovement, IConfiguration configuration, IScreenshotService screenshotService)
		{
			_mouseMovement = mouseMovement;
			_configuration = configuration;
			_screenshotService = screenshotService;
			_colorComparator = new ToleranceColorComparator(2);
		}



		private static Point FindClosestPoint(Point target, List<Point> points)
		{
			Point closestPoint = points[0];
			double closestDistance = double.MaxValue;

			foreach (Point point in points)
			{
				double distance = Math.Sqrt(Math.Pow(target.X - point.X, 2) + Math.Pow(target.Y - point.Y, 2));
				if (distance < closestDistance)
				{
					closestDistance = distance;
					closestPoint = point;
				}
			}

			return closestPoint;
		}

		/// <summary>
		/// Clicks on the determined station.
		/// </summary>
		//TODO Add check to see if a bubble is open first (removes need for triple click)
		public void StartStation()
		{
			MainForm.SetLabel(MainForm.Labels.Status, "Looking for station", Color.Orange);
			Debug.WriteLine("Looking for station");
			var screenShot = _screenshotService.CaptureScreen(_configuration.PpWindow);
			var stations = screenShot.GetAllOccurences(Resources.ui_WorkAtShoppe.ToLockedBitmap(), _colorComparator).ToList();
			var centerPoint = new Point(320, 285);
			if (stations.Count == 0)
				return;
			var closestPoint = FindClosestPoint(centerPoint, stations);
			if (!_mouseMovement.HumanMoveMouseMovement(new Point(closestPoint.X + _random.Next(13, 57), closestPoint.Y + _random.Next(-5, 19))))
				Thread.Sleep(3000);
			else
			{
				_mouseMovement.DoubleLeftClick();
				Thread.Sleep(_random.Next(97,450));
			}
		}

		public void ClickPlayButton()
		{
			var screenShot = _screenshotService.CaptureScreen(_configuration.PpWindow);

			Point buttonLocation;
			if (IsPlayButton(screenShot, out buttonLocation))
			{
				ClickPlayButton(buttonLocation);
				Thread.Sleep(_random.Next(100, 400));
			}

		}

		public bool IsPlayButton(LockedBitmap workingScreen, out Point buttonLocation)
		{
			buttonLocation = FindBitmap(Resources.playButtonImage.ToLockedBitmap(), workingScreen, 2);
			if ((buttonLocation.X == 0) && (buttonLocation.Y == 0))
			{
				return false;
			}
			return true;
		}

		private void ClickPlayButton(Point buttonLocation)
		{
			var clickLoc = new Point(buttonLocation.X + _random.Next(2, 17), buttonLocation.Y + _random.Next(0, 7));
			if (!_mouseMovement.HumanMoveMouseMovement(clickLoc))
			{
				Thread.Sleep(3000);
			}
			else
			{
				Thread.Sleep(_random.Next(10, 30));
				_mouseMovement.LeftClick();
			}
		}

		public void ExitPuzzle()
		{
			MainForm.SetLabel(Labels.Status, "Exiting puzzle");
			Debug.WriteLine("Exiting Puzzle");

			var screenShot = _screenshotService.CaptureScreen(_configuration.PpWindow);

			Point buttonLocation;
			if (IsDismissButton(screenShot, out buttonLocation))
			{
				ClickDismissPuzzle(buttonLocation);
				Thread.Sleep(_random.Next(1072, 1934));
			}

			screenShot = _screenshotService.CaptureScreen(_configuration.PpWindow);
			if (IsLeaveGameButton(screenShot, out buttonLocation))
			{
				ClickLeavePuzzle(buttonLocation);
				Thread.Sleep(_random.Next(200,600));
			}
			screenShot.Dispose();
			Debug.WriteLine("Exited");
		}

		public void PlayAgain()
		{
			var screenShot = _screenshotService.CaptureScreen(_configuration.PpWindow);

			Point buttonLocation;
			if (IsPlayAgainButton(screenShot, out buttonLocation))
			{
				MainForm.SetLabel(Labels.Status, "Starting new game");
				Debug.WriteLine("Exiting Puzzle");
				ClickPlayAgainPuzzle(buttonLocation);
				Thread.Sleep(_random.Next(100, 400));
			}
			
			screenShot.Dispose();
			Debug.WriteLine("Exited");
		}

		public bool CheckForPlayAgain()
		{
			var screenShot = _screenshotService.CaptureScreen(_configuration.PpWindow);
			return IsPlayAgainButton(screenShot, out var _);
		}



		private bool IsDismissButton(LockedBitmap workingScreen, out Point buttonLocation)
		{
			buttonLocation = FindBitmap(Resources.ui_button_Dismiss.ToLockedBitmap(), workingScreen, PuzzlingPanel.ButtonsArea, 2);
			if ((buttonLocation.X == 0) && (buttonLocation.Y == 0))
			{
				return false;
			}
			return true;
		}

		private bool IsPlayAgainButton(LockedBitmap workingScreen, out Point buttonLocation)
		{
			buttonLocation = FindBitmap(Resources.ui_PlayAgain.ToLockedBitmap(), workingScreen, 2);
			if ((buttonLocation.X == 0) && (buttonLocation.Y == 0))
			{
				return false;
			}
			return true;
		}

		private bool IsBackToDeckButton(LockedBitmap workingScreen, out Point buttonLocation)
		{
			buttonLocation = FindBitmap(Resources.ui_button_BackToDeck.ToLockedBitmap(), workingScreen, PuzzlingPanel.PanelArea, 2);
			if ((buttonLocation.X == 0) && (buttonLocation.Y == 0))
			{
				return false;
			}
			return true;
		}


		private bool IsLeaveGameButton(LockedBitmap workingScreen, out Point buttonLocation)
		{
			buttonLocation = FindBitmap(Resources.ui_Confirm_Dismiss.ToLockedBitmap(), workingScreen, PuzzlingPanel.PanelArea, 2);
			if ((buttonLocation.X == 0) && (buttonLocation.Y == 0))
			{
				return false;
			}
			return true;
		}

		private void ClickPlayAgainPuzzle(Point buttonLocation)
		{
			var clickLoc = new Point(buttonLocation.X + _random.Next(2, 57), buttonLocation.Y + _random.Next(2, 7));
			if (!_mouseMovement.HumanMoveMouseMovement(clickLoc))
			{
				Thread.Sleep(3000);
			}
			else
			{
				Thread.Sleep(_random.Next(10, 30));
				_mouseMovement.LeftClick();
			}
		}


		private bool IsAbandonButton(LockedBitmap workingScreen, out Point buttonLocation)
		{
			buttonLocation = FindBitmap(Resources.ui_button_AbandonDuty.ToLockedBitmap(), workingScreen, PuzzlingPanel.ButtonsArea, 2);
			if ((buttonLocation.X == 0) && (buttonLocation.Y == 0))
			{
				return false;
			}
			return true;
		}

		private void ClickDismissPuzzle(Point buttonLocation)
		{
			var clickLoc = new Point(buttonLocation.X + _random.Next(1, 44), buttonLocation.Y + _random.Next(1, 8));
			if (!_mouseMovement.HumanMoveMouseMovement(clickLoc))
			{
				Thread.Sleep(3000);
			}
			else
			{
				Thread.Sleep(_random.Next(10, 30));
				_mouseMovement.LeftClick();
			}
		}

		private void ClickLeavePuzzle(Point buttonLocation)
		{
			var clickLoc = new Point(buttonLocation.X + _random.Next(1, 74), buttonLocation.Y + _random.Next(2, 10));
			if (!_mouseMovement.HumanMoveMouseMovement(clickLoc))
			{
				Thread.Sleep(3000);
			}
			else
			{
				Thread.Sleep(_random.Next(10, 30));
				_mouseMovement.LeftClick();
			}
		}



		private void ClickAbandonPuzzle(Point buttonLocation)
		{
			var clickLoc = new Point(buttonLocation.X + _random.Next(1, 83), buttonLocation.Y + _random.Next(1, 9));
			if (!_mouseMovement.HumanMoveMouseMovement(clickLoc))
			{
				Thread.Sleep(3000);
			}
			Thread.Sleep(_random.Next(10, 30));
			_mouseMovement.LeftClick();
		}

		private void ClickBackToDeckPuzzle(Point buttonLocation)
		{
			var clickLoc = new Point(buttonLocation.X + _random.Next(1, 72), buttonLocation.Y + _random.Next(1, 9));
			if (!_mouseMovement.HumanMoveMouseMovement(clickLoc))
			{
				Thread.Sleep(3000);
			}
			Thread.Sleep(_random.Next(10, 30));
			_mouseMovement.LeftClick();
		}


	}

	internal static class PuzzlingPanel
	{
		public static readonly RECT ButtonsArea = new RECT(456, 260, 790, 360);
		public static readonly RECT LoadedCountArea = new RECT(778, 253, 797, 267);
		public static readonly RECT PanelArea = new RECT(450, 310, 797, 571);
	}

	internal struct RECT
	{
		public int X1;
		public int Y1;
		public int X2;
		public int Y2;

		public RECT(int X1, int Y1, int X2, int Y2)
		{
			this.X1 = X1;
			this.Y1 = Y1;
			this.X2 = X2;
			this.Y2 = Y2;
		}
	}
}
