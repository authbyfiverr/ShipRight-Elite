using LockedBitmapUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShipRight
{
	internal static class Extensions
	{
		private static Random _random = new Random();
		private static readonly Stopwatch SleepStopWatch = new Stopwatch();

		///<summary>
		///High precision sleep.
		///</summary>
		/// <param name="time">Time in seconds to sleep.</param>
		public static void Sleep(int time)
		{
			SleepStopWatch.Restart();
			while (SleepStopWatch.ElapsedMilliseconds < time) ;
			SleepStopWatch.Stop();
		}

		///<summary>
		///Deep clones a jagged array.
		///</summary>
		/// <param name="CurrentArray">The array to be cloned.</param>
		/// <returns>The cloned array.</returns>
		public static T[][] DeepClone<T>(this T[][] CurrentArray)
		{
			var NewArray = new T[CurrentArray.Length][];
			for (int i = 0; i < CurrentArray.Length; i++)
			{
				NewArray[i] = (T[])CurrentArray[i].Clone();
			}
			return NewArray;
		}

		///<summary>
		///Adds two points together.
		///</summary>
		/// <param name="p1">The current point.</param>
		/// <param name="p2">The point to add.</param>
		/// <returns>The sum of both points.</returns>
		public static Point Add(this Point p1, Point p2)
		{
			return new Point(p1.X + p2.X, p1.Y + p2.Y);
		}

		public static RECT Add(this RECT rect, Point p)
		{
			return new RECT(
				rect.X1 + p.X, rect.Y1 + p.Y, 
				rect.X2 + p.X, rect.Y2 + p.Y);


		}

		///<summary>
		///Subtracts one point from the other.
		///</summary>
		/// <param name="p1">The current point.</param>
		/// <param name="p2">The point amount to subtract.</param>
		/// <returns><paramref name="p1"/> minus <paramref name="p2"/>.</returns>
		public static Point Subtract(this Point p1, Point p2)
		{
			return new Point(p1.X - p2.X, p1.Y - p2.Y);
		}

		///<summary>
		///Adds an integer value to the X point value.
		///</summary>
		/// <param name="point">The current point.</param>
		/// <param name="value">The X value to add.</param>
		/// <returns>(<paramref name="point"/>.X + <paramref name="value"/>, <paramref name="point"/>.Y).</returns>
		public static Point AddX(this Point point, int value)
		{
			return new Point(point.X + value, point.Y);
		}

		///<summary>
		///Adds an integer value to the Y point value.
		///</summary>
		/// <param name="point">The current point.</param>
		/// <param name="value">The Y value to add.</param>
		/// <returns>(<paramref name="point"/>.X, <paramref name="point"/>.Y + <paramref name="value"/>).</returns>
		public static Point AddY(this Point point, int value)
		{
			return new Point(point.X, point.Y + value);
		}

		///<summary>
		///Subtracts an integer value from the X point value.
		///</summary>
		/// <param name="point">The current point.</param>
		/// <param name="value">The X value to subtract.</param>
		/// <returns>(<paramref name="point"/>.X + <paramref name="value"/>, <paramref name="point"/>.Y).</returns>
		public static Point SubtractX(this Point point, int value)
		{
			return new Point(point.X - value, point.Y);
		}

		///<summary>
		///Subtracts an integer value from the Y point value.
		///</summary>
		/// <param name="point">The current point.</param>
		/// <param name="value">The Y value to subtract.</param>
		/// <returns>(<paramref name="point"/>.X, <paramref name="point"/>.Y - <paramref name="value"/>).</returns>
		public static Point SubtractY(this Point point, int value)
		{
			return new Point(point.X, point.Y - value);
		}

        public static bool ToleranceEquals(this Point p1, Point p2, int tolerance)
        {
            int deltaX = Math.Abs(p1.X - p2.X);
            int deltaY = Math.Abs(p1.Y - p2.Y);

            return (deltaX <= tolerance) && (deltaY <= tolerance);
        }


        public static bool IsPngEqual(this string filePath1, string filePath2)
		{
			byte[] file1 = File.ReadAllBytes(filePath1);
			byte[] file2 = File.ReadAllBytes(filePath2);

			if (file1.Length != file2.Length)
			{
				return false;
			}

			for (int i = 0; i < file1.Length; i++)
			{
				if (file1[i] != file2[i])
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Gets the path to the Puzzle Pirate folder within AppData.
		/// </summary>
		/// <returns>The path to Puzzle Pirates AppData.</returns>
		public static string PPFolderPath()
		{
			var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			var puzzlePiratesFolder = Path.Combine(appDataFolder, "Three Rings Design", "Puzzle Pirates");
			return puzzlePiratesFolder;
		}

		//TODO Update this later to choose the folder
		public static string PPGunningFolder()
		{
			var puzzleDutyGunneryFolder = Path.Combine(PPFolderPath(),
				"rsrc", "bundles", "media", "yohoho-puzzle-duty-gunnery", "media", "yohoho", "puzzle", "duty", "gunnery");
			return puzzleDutyGunneryFolder;
		}

		//SimilarColors
		public static bool SimilarColors(int R1, int G1, int B1, int R2, int G2, int B2, int Tolerance)
		{
			bool returnValue = true;
			if (Math.Abs(R1 - R2) > Tolerance || Math.Abs(G1 - G2) > Tolerance || Math.Abs(B1 - B2) > Tolerance)
			{
				returnValue = false;
			}
			return returnValue;
		}

		public static int GetDistance(int x1, int y1, int x2, int y2)
		{
			return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
		}

		public static int GetDistance(Point p1, Point p2)
		{
			return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
		}

		public static Point Randomize(this Point p, int strayDistance )
		{
			return p.Add(new Point(
				_random.Next(-strayDistance, strayDistance),
				_random.Next(-strayDistance, strayDistance))
			);
		}


		public static (int, int) FindClosestInteger(int[][] array, int targetRow, int targetCol, List<int> integers)
		{
			double minDistance = double.MaxValue;
			int closestRow = -1;
			int closestCol = -1;

			for (int row = 0; row < array.Length; row++)
			{
				for (int col = 0; col < array[row].Length; col++)
				{
					if (integers.Contains(array[row][col]))
					{
						double distance = Math.Sqrt(Math.Pow(row - targetRow, 2) + Math.Pow(col - targetCol, 2));
						if (distance < minDistance)
						{
							minDistance = distance;
							closestRow = row;
							closestCol = col;
						}
					}
				}
			}

			return (closestRow, closestCol);
		}

		#region FindBitmap
		//FindBitmap
		public static Point FindBitmap(Bitmap bmpNeedle, LockedBitmap bmpHaystack)
		{
			Color colHay = bmpHaystack.GetPixel(0, 0);
			Color colNeedle = bmpNeedle.GetPixel(0, 0);
			for (int x1 = 0; x1 < bmpHaystack.Width - bmpNeedle.Width; x1++)
			{
				for (int y1 = 0; y1 < bmpHaystack.Height - bmpNeedle.Height; y1++)
				{
					colHay = bmpHaystack.GetPixel(x1, y1);
					colNeedle = bmpNeedle.GetPixel(0, 0);
					if ((colHay.R == colNeedle.R) && (colHay.G == colNeedle.G) && (colHay.B == colNeedle.B))
					{
						for (int x2 = 1; x2 < bmpNeedle.Width; x2++)
						{
							for (int y2 = 1; y2 < bmpNeedle.Height; y2++)
							{
								colHay = bmpHaystack.GetPixel(x1 + x2, y1 + y2);
								colNeedle = bmpNeedle.GetPixel(x2, y2);
								if (!((colHay.R == colNeedle.R) && (colHay.G == colNeedle.G) && (colHay.B == colNeedle.B)))
								{
									goto nextLoop;
								}
							}
						}
						return new Point(x1, y1);
					nextLoop:
						continue;
					}
				}
			}
			return new Point(0, 0);
		}

		//FindBitmap [Optional - Tolerance]
		public static Point FindBitmap(LockedBitmap bmpNeedle, LockedBitmap bmpHaystack, int Tolerance)
		{
			Color colHay = bmpHaystack.GetPixel(0, 0);
			Color colNeedle = bmpNeedle.GetPixel(0, 0);
			for (int x1 = 0; x1 < bmpHaystack.Width - bmpNeedle.Width; x1++)
			{
				for (int y1 = 0; y1 < bmpHaystack.Height - bmpNeedle.Height; y1++)
				{
					colHay = bmpHaystack.GetPixel(x1, y1);
					colNeedle = bmpNeedle.GetPixel(0, 0);
					if (SimilarColors(colHay.R, colHay.G, colHay.B, colNeedle.R, colNeedle.G, colNeedle.B, Tolerance))
					{
						for (int x2 = 1; x2 < bmpNeedle.Width; x2++)
						{
							for (int y2 = 1; y2 < bmpNeedle.Height; y2++)
							{
								colHay = bmpHaystack.GetPixel(x1 + x2, y1 + y2);
								colNeedle = bmpNeedle.GetPixel(x2, y2);
								if (!(SimilarColors(colHay.R, colHay.G, colHay.B, colNeedle.R, colNeedle.G, colNeedle.B, Tolerance)))
								{
									goto nextLoop;
								}
							}
						}
						return new Point(x1, y1);
					nextLoop:
						continue;
					}
				}
			}
			return new Point(0, 0);
		}

		//FindBitmap [Optional - RECT]
		public static Point FindBitmap(Bitmap bmpNeedle, LockedBitmap bmpHaystack, RECT rect)
		{
			Color colHay = bmpHaystack.GetPixel(0, 0);
			Color colNeedle = bmpNeedle.GetPixel(0, 0);
			for (int x1 = (rect.X1 - 1); x1 < (rect.X2 + 1) - bmpNeedle.Width; x1++)
			{
				for (int y1 = (rect.Y1 - 1); y1 < (rect.Y2 + 1) - bmpNeedle.Height; y1++)
				{
					colHay = bmpHaystack.GetPixel(x1, y1);
					colNeedle = bmpNeedle.GetPixel(0, 0);
					if ((colHay.R == colNeedle.R) && (colHay.G == colNeedle.G) && (colHay.B == colNeedle.B))
					{
						for (int x2 = 1; x2 < bmpNeedle.Width; x2++)
						{
							for (int y2 = 1; y2 < bmpNeedle.Height; y2++)
							{
								colHay = bmpHaystack.GetPixel(x1 + x2, y1 + y2);
								colNeedle = bmpNeedle.GetPixel(x2, y2);
								if (!((colHay.R == colNeedle.R) && (colHay.G == colNeedle.G) && (colHay.B == colNeedle.B)))
								{
									goto nextLoop;
								}
							}
						}
						return new Point(x1, y1);
					nextLoop:
						continue;
					}
				}
			}
			return new Point(0, 0);
		}

		//FindBitmap [Optional - RECT, Tolerance]
		public static Point FindBitmap(LockedBitmap bmpNeedle, LockedBitmap bmpHaystack, RECT rect, int Tolerance)
		{
			Color colHay = bmpHaystack.GetPixel(0, 0);
			Color colNeedle = bmpNeedle.GetPixel(0, 0);
			for (int x1 = (rect.X1 - 1); x1 < (rect.X2 + 1) - bmpNeedle.Width; x1++)
			{
				for (int y1 = (rect.Y1 - 1); y1 < (rect.Y2 + 1) - bmpNeedle.Height; y1++)
				{
					colHay = bmpHaystack.GetPixel(x1, y1);
					colNeedle = bmpNeedle.GetPixel(0, 0);
					if (SimilarColors(colHay.R, colHay.G, colHay.B, colNeedle.R, colNeedle.G, colNeedle.B, Tolerance))
					{
						for (int x2 = 1; x2 < bmpNeedle.Width; x2++)
						{
							for (int y2 = 1; y2 < bmpNeedle.Height; y2++)
							{
								colHay = bmpHaystack.GetPixel(x1 + x2, y1 + y2);
								colNeedle = bmpNeedle.GetPixel(x2, y2);
								if (!(SimilarColors(colHay.R, colHay.G, colHay.B, colNeedle.R, colNeedle.G, colNeedle.B, Tolerance)))
								{
									goto nextLoop;
								}
							}
						}
						return new Point(x1, y1);
					nextLoop:
						continue;
					}
				}
			}
			return new Point(0, 0);
		}

		//FindBitmap [Optional - RECT, Tolerance, NeedleAreaWidth, NeedleAreaHeight]
		public static Point FindBitmap(LockedBitmap bmpNeedle, LockedBitmap bmpHaystack, RECT rect, int Tolerance, int needleAreaWidth, int needleAreaHeight)
		{
			Color colHay = bmpHaystack.GetPixel(0, 0);
			Color colNeedle = bmpNeedle.GetPixel(0, 0);
			for (int x1 = (rect.X1 - 1); x1 < (rect.X2 + 1) - bmpNeedle.Width; x1++)
			{
				for (int y1 = (rect.Y1 - 1); y1 < (rect.Y2 + 1) - bmpNeedle.Height; y1++)
				{
					colHay = bmpHaystack.GetPixel(x1, y1);
					colNeedle = bmpNeedle.GetPixel(0, 0);
					if (SimilarColors(colHay.R, colHay.G, colHay.B, colNeedle.R, colNeedle.G, colNeedle.B, Tolerance))
					{
						for (int x2 = 1; x2 < needleAreaWidth; x2++)
						{
							for (int y2 = 1; y2 < needleAreaHeight; y2++)
							{
								colHay = bmpHaystack.GetPixel(x1 + x2, y1 + y2);
								colNeedle = bmpNeedle.GetPixel(x2, y2);
								if (!(SimilarColors(colHay.R, colHay.G, colHay.B, colNeedle.R, colNeedle.G, colNeedle.B, Tolerance)))
								{
									goto nextLoop;
								}
							}
						}
						return new Point(x1, y1);
						nextLoop:
						continue;
					}
				}
			}
			return new Point(0, 0);
		}

		//FindBitmap [Optional - X1, Y1, X2, Y2]
		public static Point FindBitmap(Bitmap bmpNeedle, LockedBitmap bmpHaystack, int X1, int Y1, int X2, int Y2)
		{
			RECT rect;
			rect.X1 = X1;
			rect.Y1 = Y1;
			rect.X2 = X2;
			rect.Y2 = Y2;
			Color colHay = bmpHaystack.GetPixel(0, 0);
			Color colNeedle = bmpNeedle.GetPixel(0, 0);
			for (int x1 = (rect.X1 - 1); x1 < (rect.X2 + 1) - bmpNeedle.Width; x1++)
			{
				for (int y1 = (rect.Y1 - 1); y1 < (rect.Y2 + 1) - bmpNeedle.Height; y1++)
				{
					colHay = bmpHaystack.GetPixel(x1, y1);
					colNeedle = bmpNeedle.GetPixel(0, 0);
					if ((colHay.R == colNeedle.R) && (colHay.G == colNeedle.G) && (colHay.B == colNeedle.B))
					{
						for (int x2 = 1; x2 < bmpNeedle.Width; x2++)
						{
							for (int y2 = 1; y2 < bmpNeedle.Height; y2++)
							{
								colHay = bmpHaystack.GetPixel(x1 + x2, y1 + y2);
								colNeedle = bmpNeedle.GetPixel(x2, y2);
								if (!((colHay.R == colNeedle.R) && (colHay.G == colNeedle.G) && (colHay.B == colNeedle.B)))
								{
									goto nextLoop;
								}
							}
						}
						return new Point(x1, y1);
					nextLoop:
						continue;
					}
				}
			}
			return new Point(0, 0);
		}

		//FindBitmap [Optional - X1, Y1, X2, Y2, Mask]
		public static Point FindBitmap(Bitmap bmpNeedle, LockedBitmap bmpHaystack, int X1, int Y1, int X2, int Y2, Color Mask)
		{
			RECT rect;
			rect.X1 = X1;
			rect.Y1 = Y1;
			rect.X2 = X2;
			rect.Y2 = Y2;
			Color colHay = bmpHaystack.GetPixel(0, 0);
			Color colNeedle = bmpNeedle.GetPixel(0, 0);
			for (int x1 = (rect.X1 - 1); x1 < (rect.X2 + 1) - bmpNeedle.Width; x1++)
			{
				for (int y1 = (rect.Y1 - 1); y1 < (rect.Y2 + 1) - bmpNeedle.Height; y1++)
				{
					colHay = bmpHaystack.GetPixel(x1, y1);
					colNeedle = bmpNeedle.GetPixel(0, 0);
					if ((colHay.R == colNeedle.R) && (colHay.G == colNeedle.G) && (colHay.B == colNeedle.B))
					{
						for (int x2 = 0; x2 < bmpNeedle.Width; x2++)
						{
							for (int y2 = 0; y2 < bmpNeedle.Height; y2++)
							{
								colHay = bmpHaystack.GetPixel(x1 + x2, y1 + y2);
								colNeedle = bmpNeedle.GetPixel(x2, y2);
								if (colNeedle.R == Mask.R && colNeedle.G == Mask.G && colNeedle.B == Mask.B)
									goto NextPixel;
								if (!((colHay.R == colNeedle.R) && (colHay.G == colNeedle.G) && (colHay.B == colNeedle.B)))
								{
									goto nextLoop;
								}
							NextPixel:
								continue;
							}
						}
						return new Point(x1, y1);
					nextLoop:
						continue;
					}
				}
			}
			return new Point(0, 0);
		}

		//FindBitmap [Optional - X1, Y1, X2, Y2, Tolerance]
		public static Point FindBitmap(Bitmap bmpNeedle, LockedBitmap bmpHaystack, int X1, int Y1, int X2, int Y2, int Tolerance)
		{
			RECT rect;
			rect.X1 = X1;
			rect.Y1 = Y1;
			rect.X2 = X2;
			rect.Y2 = Y2;
			Color colHay = bmpHaystack.GetPixel(0, 0);
			Color colNeedle = bmpNeedle.GetPixel(0, 0);
			for (int x1 = (rect.X1 - 1); x1 < (rect.X2 + 1) - bmpNeedle.Width; x1++)
			{
				for (int y1 = (rect.Y1 - 1); y1 < (rect.Y2 + 1) - bmpNeedle.Height; y1++)
				{
					colHay = bmpHaystack.GetPixel(x1, y1);
					colNeedle = bmpNeedle.GetPixel(0, 0);
					if (SimilarColors(colHay.R, colHay.G, colHay.B, colNeedle.R, colNeedle.G, colNeedle.B, Tolerance))
					{
						for (int x2 = 1; x2 < bmpNeedle.Width; x2++)
						{
							for (int y2 = 1; y2 < bmpNeedle.Height; y2++)
							{
								colHay = bmpHaystack.GetPixel(x1 + x2, y1 + y2);
								colNeedle = bmpNeedle.GetPixel(x2, y2);
								if (!(SimilarColors(colHay.R, colHay.G, colHay.B, colNeedle.R, colNeedle.G, colNeedle.B, Tolerance)))
								{
									goto nextLoop;
								}
							}
						}
						return new Point(x1, y1);
					nextLoop:
						continue;
					}
				}
			}
			return new Point(0, 0);
		}

		//FindBitmap [Optional - Rectangle, Tolerance]
		public static Point FindBitmap(LockedBitmap bmpNeedle, LockedBitmap bmpHaystack, Rectangle rectangle, int Tolerance)
		{
			RECT rect;
			rect.X1 = rectangle.X;
			rect.Y1 = rectangle.Y;
			rect.X2 = rectangle.Right;
			rect.Y2 = rectangle.Bottom;
			Color colHay = bmpHaystack.GetPixel(0, 0);
			Color colNeedle = bmpNeedle.GetPixel(0, 0);
			for (int x1 = (rect.X1 - 1); x1 < (rect.X2 + 1) - bmpNeedle.Width; x1++)
			{
				for (int y1 = (rect.Y1 - 1); y1 < (rect.Y2 + 1) - bmpNeedle.Height; y1++)
				{
					colHay = bmpHaystack.GetPixel(x1, y1);
					colNeedle = bmpNeedle.GetPixel(0, 0);
					if (SimilarColors(colHay.R, colHay.G, colHay.B, colNeedle.R, colNeedle.G, colNeedle.B, Tolerance))
					{
						for (int x2 = 1; x2 < bmpNeedle.Width; x2++)
						{
							for (int y2 = 1; y2 < bmpNeedle.Height; y2++)
							{
								colHay = bmpHaystack.GetPixel(x1 + x2, y1 + y2);
								colNeedle = bmpNeedle.GetPixel(x2, y2);
								if (!(SimilarColors(colHay.R, colHay.G, colHay.B, colNeedle.R, colNeedle.G, colNeedle.B, Tolerance)))
								{
									goto nextLoop;
								}
							}
						}
						return new Point(x1, y1);
					nextLoop:
						continue;
					}
				}
			}
			return new Point(0, 0);
		}


		#endregion


		public static bool AreJaggedArraysEqual(int[][] array1, int[][] array2)
		{
			if (array1 == null || array2 == null)
				return array1 == array2;

			if (array1.Length != array2.Length)
				return false;

			for (int i = 0; i < array1.Length; i++)
			{
				if (array1[i] == null || array2[i] == null)
				{
					if (array1[i] != array2[i])
						return false;
					continue;
				}

				if (array1[i].Length != array2[i].Length)
					return false;

				for (int j = 0; j < array1[i].Length; j++)
				{
					if (array1[i][j] != array2[i][j])
						return false;
				}
			}

			return true;
		}




	}
}
