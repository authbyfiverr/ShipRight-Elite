using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ShipRight.Extensions;

namespace ShipRight
{
	//Version 1.1.0
	internal class MouseMovement : IMouseMovement
	{
		private readonly IConfiguration _configuration;
		private readonly Random _random;
		private readonly Stopwatch _sleepStopWatch = new Stopwatch();


		private const int MOUSEEVENTF_LEFTDOWN = 0x02;
		private const int MOUSEEVENTF_LEFTUP = 0x04;
		private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
		private const int MOUSEEVENTF_RIGHTUP = 0x10;

		private const int VK_LBUTTON = 0x01;

		// Constants for the key event flags
		private const int KEYEVENTF_KEYDOWN = 0x1;
		private const int KEYEVENTF_KEYUP = 0x2;



		public MouseMovement(IConfiguration configuration)
		{
			_configuration = configuration;
			_random = new Random();
		}

		private static int _dp = 0;
		private static List<double> mouseSpeeds = new List<double>();

		
		public bool HumanMoveMouseMovement(System.Drawing.Point endLocation, int speed = 5, bool hmm = true)
		{
			speed = _configuration.MouseSpeed;
			var originPoint = WindowsInterface.GetClientPosition(_configuration.PpWindow);
			endLocation = new Point(endLocation.X + originPoint.X, endLocation.Y + originPoint.Y);



			//Debug.WriteLine($"Speed: {speed}");
			if (hmm)
			{
				speed = Math.Clamp(_configuration.MouseSpeed, 1, 10);
				int sleepDuration = 20 - (2 * speed);
				if (sleepDuration <= 0) { sleepDuration = 1; }


				Stopwatch sw = new Stopwatch();

				sw.Start();
				detectionBypass(sleepDuration);

				System.Drawing.Point CursorPos = System.Windows.Forms.Cursor.Position;
				var distance = Convert.ToInt32(Math.Sqrt(Math.Pow(endLocation.X - CursorPos.X, 2) +
				                                         Math.Pow(endLocation.Y - CursorPos.Y, 2)));

				var distanceSpeed = distance * (1 - (speed / 10));
				var numOfPoint = distanceSpeed;

				numOfPoint = (numOfPoint / speed);

				List<System.Drawing.Point> b = GenerateCurve(endLocation, numOfPoint);

				Point point2 = new Point(-1, -1);
				

				for (int i = 0; i < b.Count; i++)
				{
					if (point2.X != -1 && !point2.ToleranceEquals(Cursor.Position, 20))
					{
						Debug.WriteLine($"Mouse interrupted. Expected {point2} but mouse is at {Cursor.Position}");
						return false;
					}
					System.Windows.Forms.Cursor.Position = b[i];
					point2 = Cursor.Position;
					Thread.Sleep(sleepDuration);
				}

				Cursor.Position = endLocation;
				sw.Stop();
				double actualSpeed = distance / (double)sw.ElapsedMilliseconds;
				Console.WriteLine($"Actual Speed {actualSpeed}");
				mouseSpeeds.Add(actualSpeed);
				var averageSpeed = mouseSpeeds.Average();
				Debug.WriteLine(mouseSpeeds.Average());
				if (averageSpeed >= 0.7)
					MainForm.SetLabel(MainForm.Labels.MouseSpeed, $"{speed,0:0.000}", Color.DarkRed);
				else if (averageSpeed <= 0.2)
					MainForm.SetLabel(MainForm.Labels.MouseSpeed, $"{speed,0:0.000}", Color.DarkBlue);
				else
					MainForm.SetLabel(MainForm.Labels.MouseSpeed,$"{averageSpeed,0:0.000}");
                CheckMouseBounds(averageSpeed);
            }
			else
				System.Windows.Forms.Cursor.Position = endLocation;

			return true;
		}

        private void CheckMouseBounds(double speed)
        {
            if (mouseSpeeds.Count > 10)
            {
                mouseSpeeds.RemoveRange(0, mouseSpeeds.Count - 10);
            }
            
            speed = Math.Round(speed, 2);

            if (speed >= 0.7D)
            {
				UpdateMouseSpeed(false);
				MainForm.SetLabel(MainForm.Labels.MouseSpeed, $"{speed,0:0.000}", Color.DarkRed);
				Debug.WriteLine("SPEED IS FLAGGING");
            }
            else if (speed <= 0.2D)
            {
				UpdateMouseSpeed(true);
				MainForm.SetLabel(MainForm.Labels.MouseSpeed, $"{speed,0:0.000}", Color.DarkBlue);
			}
        }

        private void UpdateMouseSpeed(bool up)
        {
	        if (_configuration.MouseSpeed > 9 || _configuration.MouseSpeed < 1)
	        {
		        return;
	        }

			if (up)
				_configuration.MouseSpeed++;
			else
				_configuration.MouseSpeed--;

			Properties.Settings.Default.MouseSpeed = _configuration.MouseSpeed;
			Properties.Settings.Default.Save();
			mouseSpeeds.Clear();
        }


        private static List<System.Drawing.Point> GenerateCurve(System.Drawing.Point endLocation, int numSteps = 100)
		{
			List<System.Drawing.Point> curvePoints = new List<System.Drawing.Point>();
			System.Drawing.Point CursorPos = System.Windows.Forms.Cursor.Position;
			List<System.Drawing.Point> controlPoints = new List<System.Drawing.Point>();
			controlPoints.Add(CursorPos);
			controlPoints.Add(getRandomPoint(CursorPos, endLocation, 1));
			controlPoints.Add(getRandomPoint(CursorPos, endLocation, 2));
			controlPoints.Add(getRandomPoint(CursorPos, endLocation, 3));
			controlPoints.Add(endLocation);
			double stepSize = 1.0 / numSteps;
			for (double t = 0; t <= 1; t += stepSize)
			{
				curvePoints.Add(CalculateBezierPoint(t, controlPoints));
			}

			return curvePoints;
		}

		private static System.Drawing.Point CalculateBezierPoint(double t, List<System.Drawing.Point> controlPoints)
		{
			double x = 0;
			double y = 0;
			int n = controlPoints.Count - 1;
			for (int i = 0; i <= n; i++)
			{
				double bernstein = CalculateBernstein(n, i, t);
				x += controlPoints[i].X * bernstein;
				y += controlPoints[i].Y * bernstein;
			}

			return new System.Drawing.Point((int)x, (int)y);
		}

		private static double CalculateBernstein(int n, int i, double t)
		{
			double binomialCoeff = CalculateBinomialCoefficient(n, i);
			double t1 = Math.Pow(t, i);
			double t2 = Math.Pow((1 - t), (n - i));
			return binomialCoeff * t1 * t2;
		}

		private static double CalculateBinomialCoefficient(int n, int k)
		{
			double result = 1;
			for (int i = 1; i <= k; i++)
			{
				result *= (n - (k - i)) / (double)i;
			}

			return result;
		}

		private static System.Drawing.Point getRandomPoint(System.Drawing.Point start, System.Drawing.Point end,
			int section)
		{
			System.Drawing.Point ranP = new System.Drawing.Point(0, 0);

			int distx = Convert.ToInt32(start.X - end.X);
			int disty = Convert.ToInt32(start.Y - end.Y);

			if (section == 1)
			{
				ranP.X = (ranNumber(Convert.ToInt32(start.X),
					ranNumber(Convert.ToInt32(start.X), Convert.ToInt32(start.X + (distx * 0.3f)))));
				ranP.Y = (ranNumber(Convert.ToInt32(start.Y),
					ranNumber(Convert.ToInt32(start.Y), Convert.ToInt32(start.Y + (disty * 0.3f)))));
			}
			else if (section == 2)
			{
				ranP.X = (ranNumber(Convert.ToInt32(start.X),
					ranNumber(Convert.ToInt32(start.X), Convert.ToInt32(start.X + (distx * 0.7f)))));
				ranP.Y = (ranNumber(Convert.ToInt32(start.Y),
					ranNumber(Convert.ToInt32(start.Y), Convert.ToInt32(start.Y + (disty * 0.7f)))));
			}
			else if (section == 3)
			{
				ranP.X = (ranNumber(Convert.ToInt32(end.X),
					ranNumber(Convert.ToInt32(end.X), Convert.ToInt32(end.X - distx / 3))));
				ranP.Y = (ranNumber(Convert.ToInt32(end.Y),
					ranNumber(Convert.ToInt32(end.Y), Convert.ToInt32(end.Y - disty / 3))));
			}

			return ranP;
		}

		private static int ranNumber(int min, int max)
		{
			Random rnd = new Random();
			if (min > max)
				return rnd.Next(max, min);
			if (min < max)
				return rnd.Next(min, max);
			else
				return min;
		}

		private static void detectionBypass(int sleepDuration)
		{
			if (_dp > 4)
				_dp = 1;

			switch (_dp)
			{
				case 1:
					for (int x = 1; x < 6; x++)
					{
						System.Windows.Forms.Cursor.Position = new System.Drawing.Point(
							System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y + x);
						Thread.Sleep(sleepDuration);

					}

					_dp++;
					return;
				case 2:
					for (int x = 1; x < 6; x++)
					{
						System.Windows.Forms.Cursor.Position = new System.Drawing.Point(
							System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y - x);
						Thread.Sleep(sleepDuration);
					}

					_dp++;
					return;
				case 3:
					for (int x = 1; x < 6; x++)
					{
						System.Windows.Forms.Cursor.Position = new System.Drawing.Point(
							System.Windows.Forms.Cursor.Position.X + x, System.Windows.Forms.Cursor.Position.Y);
						Thread.Sleep(sleepDuration);
					}

					_dp++;
					return;
				case 4:
					for (int x = 1; x < 6; x++)
					{
						System.Windows.Forms.Cursor.Position = new System.Drawing.Point(
							System.Windows.Forms.Cursor.Position.X - x, System.Windows.Forms.Cursor.Position.Y);
						Thread.Sleep(sleepDuration);
					}

					_dp++;
					return;
				default:
					return;
			}
		}


		public void ClearAverages()
		{
			mouseSpeeds.Clear();

		}

		public void PauseGame()
		{
			Thread.Sleep(_random.Next(10, 40));
			PressKey((byte)Keys.Escape);
			Thread.Sleep(_random.Next(100, 200));
		}

		public void UnPauseGame()
		{
			Thread.Sleep(_random.Next(10, 40));
			PressKey((byte)Keys.Escape);
			Thread.Sleep(_random.Next(100, 200));
		}

		public void LeftDown()
		{
			Thread.Sleep(_random.Next(10, 20));
			mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
			Thread.Sleep(_random.Next(35, 130));
		}

		public bool IsLeftPressed()
		{
			var keyState = GetAsyncKeyState(VK_LBUTTON);
			var isLeftButtonDown = (keyState & 0x8000) != 0;

			if (isLeftButtonDown)
			{
				return true;
			}

			return false;

		}

		public void LeftUp()
		{
			Thread.Sleep(_random.Next(35, 130));
			mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
			Thread.Sleep(_random.Next(10, 20));
		}

		public void LeftClick()
		{
			Thread.Sleep(_random.Next(10, 20));
			mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
			Thread.Sleep(_random.Next(35, 130));
			mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
			Thread.Sleep(_random.Next(10, 20));
		}

		public void DoubleLeftClick()
		{
			LeftClick();
			Thread.Sleep(_random.Next(50, 100));
			LeftClick();
		}

		public void RightClick()
		{
			Thread.Sleep(_random.Next(10, 20));
			mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
			Thread.Sleep(_random.Next(35, 130));
			mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
			Thread.Sleep(_random.Next(10, 20));
		}

		public void PressKey(byte keycode)
		{
			
			Thread.Sleep(_random.Next(10, 20));
			keybd_event(keycode, 0, KEYEVENTF_KEYDOWN, 0);
			Thread.Sleep(_random.Next(35, 130));
			keybd_event(keycode, 0, KEYEVENTF_KEYUP, 0);
			Thread.Sleep(_random.Next(10, 20));
		}

		public void PressKeyInstant(byte keycode)
		{
			keybd_event(keycode, 0, KEYEVENTF_KEYDOWN, 0);
			keybd_event(keycode, 0, KEYEVENTF_KEYUP, 0);
			_sleepStopWatch.Restart();
			while (_sleepStopWatch.ElapsedMilliseconds < 8) ;
		}


		[DllImport("user32.dll", SetLastError = true)]
		static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

		[DllImport("user32.dll")]
		public static extern short GetAsyncKeyState(int vKey);
	}
}
