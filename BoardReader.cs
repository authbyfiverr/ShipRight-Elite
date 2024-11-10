using LockedBitmapUtil;
using LockedBitmapUtil.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;
using static ShipRight.Extensions;
using static ShipRight.ColorExtensions;

namespace ShipRight
{
	internal class BoardReader : IBoardReader
	{
		private readonly LockedBitmap _boardReferenceImage;
		private readonly LockedBitmap _boardReferenceDRImage;
		private readonly LockedBitmap _normalBackgroundImage;
		private readonly IConfiguration _configuration;
		private readonly IScreenshotService _screenshotService;
		private readonly IColorComparator _colorComparator;
		public List<Piece> AllPieces { get; set; }
		private int _pieceFailCount { get; set; }

		public Point BoardOffset { get; set; }          //Pixel offset from corner of app to board reference.
		private Point OriginPoint { get; set; }         //Pixel location of PP
		private Point BoardRefOffset { get; set; }      //Pixel offset from board reference to start of board

		//
		// SET THESE VARIABLES BASED ON PUZZLE
		//

		//Detecting Puzzle Board
		private Color InPuzzleWindowColor = Color.FromArgb(54, 46, 42);
		private Color FlagColor = Color.FromArgb(179, 77, 65);
		private Color InPuzzleWindowDrColor = Color.FromArgb(84, 72, 66);

		#region Board Reference Image Explanation
		/*
         Board ref's must be the same size. Bottom right of the image must be the last pixel from the start of the board.
         ________
        |X X X X|
        |X X X X|
        |X X X X|
        |X_X_X_X|_ _ _ _ _ _ _
               →|X BOARD START
                |X X X X X X X
            
        

        
            |X X X X X X X
        */

		#endregion
		private readonly Bitmap _boardRef = new Bitmap(Properties.Resources.ui_BoardRef);
		private readonly Bitmap _boardDrRef = new Bitmap(Properties.Resources.ui_BoardRefDr);
		private readonly Bitmap _backgroundImage = new Bitmap(Properties.Resources._background); //Board Area Only

		//Reading Board and Pieces
		private const int BoardWidth = 5;          //Grid width(pieces)
		private const int BoardHeight = 5;          //Grid height(pieces)
		private const int GridSpacing = 60;         //Pixel spacing between pieces
		private const int PixelReadOffset = 30;     //Which pixel are we reading from the piece (X,Y) Subtract authoffset from this.

		//PixelReadOffset should be 30 but we're deducting for the offset.
		private const int Offset = 26; //dummy
		private static readonly Point BackgroundOffset = new Point(95, 52);		//Offset from background image to actual offset location

		public BoardReader(IConfiguration configuration, IScreenshotService screenshotService)
		{
			BoardRefOffset = new Point(_boardRef.Width, _boardRef.Height);
			_boardReferenceImage = _boardRef.ToLockedBitmap();
			_boardReferenceDRImage = _boardDrRef.ToLockedBitmap();
			_normalBackgroundImage = _backgroundImage.ToLockedBitmap();
			_configuration = configuration;
			_screenshotService = screenshotService;
			_colorComparator = new ToleranceColorComparator(2);
			AllPieces = new List<Piece>();
			InitializePieces();
		}

		/// <summary>
		/// Attempts to read the game board.
		/// </summary>
		/// <param name="gameBoard">The game board in int[][] format.</param>
		/// <returns>True if we successfully read the board.</returns>
		public bool TryGetBoard(out int[][] gameBoard)
		{
			using var screenshot = _screenshotService.CaptureScreen(_configuration.PpWindow);
			try
			{
				gameBoard = new int[BoardHeight][];
				for (int i = 0; i < BoardHeight; i++)
				{
					gameBoard[i] = new int[BoardWidth];
				}

				for (int y = 0; y < BoardHeight; y++)
				{
					for (int x = 0; x < BoardWidth; x++)
					{
						var pixelCoord = GetGridCenterPoint(x, y);
						var color = screenshot.GetPixel(pixelCoord.X, pixelCoord.Y);
						
						if (color.ToleranceEquals(TileColors.SAIL))
							gameBoard[y][x] = (int)Tile.SAIL;
						else if (color.ToleranceEquals(TileColors.ROPE))
							gameBoard[y][x] = (int)Tile.ROPE;
						else if (color.ToleranceEquals(TileColors.CANNON))
							gameBoard[y][x] = (int)Tile.CANNON;
						else if (color.ToleranceEquals(TileColors.WOOD))
							gameBoard[y][x] = (int)Tile.WOOD;
						else if (color.ToleranceEquals(TileColors.GOLD))
							gameBoard[y][x] = (int)Tile.GOLD;
						else if (color.ToleranceEquals(_normalBackgroundImage.GetPixel(GridSpacing * x + PixelReadOffset, GridSpacing * y + PixelReadOffset)))
						{
							//Debug.Write($"Loc: ({x}, {y}) Colour: ({color.R}, {color.G}, {color.B})");
							gameBoard[y][x] = (int)Tile.UNKNOWN;
						}

						else if (gameBoard[y][x] == default || gameBoard[y][x] == (int)Tile.UNKNOWN)
						{
							//var xCoord = 45 * x + 21 + referenceLocation.X;
							//var yCoord = 45 * y + 15 + referenceLocation.Y;

							//var bgpixel = GetBackgroundImage(screenshot).GetPixel(45 * x + 21 + referenceLocation.X,
							//45 * y + 15 + referenceLocation.Y);
							//Debug.WriteLine($"Loc: (x:{x}, y:{y}) Found color: ({color.R}, {color.G}, {color.B} at x: {xCoord} y: {yCoord})");
							//Debug.WriteLine($"Loc: (x:{x}, y:{y}) BG: ({bgpixel.R}, {bgpixel.G}, {bgpixel.B})");
							return false;
							gameBoard[y][x] = (int)Tile.UNKNOWN;
						}
					}
				}

				/*
                for (var y = 0; y < 8; y++)
                    for (var x = 0; x < 8; x++)
                        if (gameBoard[y][x] == (int)Piece.UNKNOWN) return false;
                */
				//Check board
				return true;
			}
			finally
			{
				screenshot.Dispose();
			}
		}

		

		public bool TryGetPieces(out List<Piece> currentPieces)
		{
			using var screenshot = _screenshotService.CaptureScreen(_configuration.PpWindow);
			try
			{
				currentPieces = new List<Piece>();
				
				foreach (var piece in AllPieces)
				{
					var result = FindBitmap(piece.ImageRef, screenshot, PuzzleLocations.PieceRect.Add(BoardOffset), 2);
					if (result.IsEmpty) continue;

					var index = IdentifyIndex(result);
					if (index == 99)
					{
						Debug.WriteLine("ERROR!!!");
						return false;
					}

					piece.Index = index;
					piece.IndexPoint = result;
					currentPieces.Add(piece);

					if (currentPieces.Any() && currentPieces.Count() == 6)
						break;
				}

				var found = currentPieces.Count() == 6;
				if (!found)
				{
					_pieceFailCount++;
					if (_pieceFailCount > 10)
						found = true;
				}
				else _pieceFailCount = 0;

				
				return found;

			}
			finally
			{
				screenshot.Dispose();
			}
		}

		//Get first
		public bool TryGetFlag(out int flagPos)
		{
			using var screenshot = _screenshotService.CaptureScreen(_configuration.PpWindow);
			try
			{
				for (flagPos = 0; flagPos < 21; flagPos++)
				{
					var flagPixel = PuzzleLocations.FlagZero.Add(BoardOffset).SubtractY(flagPos * _configuration.AuthValue);

					if (screenshot.GetPixel(flagPixel.X, flagPixel.Y).ToleranceEquals(FlagColor))
					{
						MainForm.SetLabel(MainForm.Labels.Flag, $"{flagPos}/20");
						return true;
					}
				}

				return false;
			}
			finally
			{
				screenshot.Dispose();
			}

		}



		public Point GetGridCenterPoint(int x, int y)
		=> new Point(GridSpacing * x + PixelReadOffset + BoardOffset.X - 12 + _configuration.AuthValue, GridSpacing * y + PixelReadOffset + BoardOffset.Y);

		public Point GetGridPoint(int x, int y)
			=> new Point(GridSpacing * x + BoardOffset.X - 12 + _configuration.AuthValue, GridSpacing * y + BoardOffset.Y);


		/// <summary>
		/// Searches for the board anchorpoint and assigns the location. This should not be a default check to determine puzzling.
		/// </summary>
		/// <param name="handle">The handle to take a screenshot from.</param>
		/// <returns>True if the reference image is found.</returns>
		public bool GetAnchorPoint(IntPtr handle)
		{
			using var screenshot = _screenshotService.CaptureScreen(handle);
			if (screenshot.DoesImageExist(_boardReferenceImage, out var anchorLocation, _colorComparator))
			{
				OriginPoint = anchorLocation;
				BoardOffset = new Point(anchorLocation.X + BoardRefOffset.X, anchorLocation.Y + BoardRefOffset.Y);
				_configuration.BoardOffset = BoardOffset;
				MainForm.SetLabel(MainForm.Labels.Status, "Board anchor set", Color.Black);
				Debug.WriteLine($"Board anchor found at {BoardOffset}");
				return true;
			}
			if (screenshot.DoesImageExist(_boardReferenceDRImage, out anchorLocation, _colorComparator))
			{
				OriginPoint = anchorLocation;
				BoardOffset = new Point(anchorLocation.X + BoardRefOffset.X, anchorLocation.Y + BoardRefOffset.Y);
				_configuration.BoardOffset = BoardOffset;
				MainForm.SetLabel(MainForm.Labels.Status, "Board anchor set", Color.Black);
				Debug.WriteLine($"Board anchor found at {BoardOffset}");
				return true;
			}
			return false;
		}

		/// <summary>
		/// Uses the a handle to take a screenshot and determine if we are currently in a puzzle.
		/// </summary>
		/// <param name="handle">The handle to take a screenshot from.</param>
		/// <param name="windowstatus">Outputs the status of DutyReport or Active.</param>
		/// <returns>True if in puzzle or </returns>
		public bool InPuzzle(IntPtr handle, out PuzzleStatus windowstatus)
		{
			using var screenshot = _screenshotService.CaptureScreen(handle);
			if (screenshot.GetPixel(OriginPoint.X, OriginPoint.Y).ToleranceEquals(InPuzzleWindowColor))
			{
				windowstatus = PuzzleStatus.Active;
				return true;
			}
			if (screenshot.GetPixel(OriginPoint.X, OriginPoint.Y).ToleranceEquals(InPuzzleWindowDrColor))
			{
				windowstatus = PuzzleStatus.DutyReport;
				return true;
			}
			
			windowstatus = PuzzleStatus.Unknown;
			return false;
		}

		/// <summary>
		/// Determines if we are currently in a puzzle.
		/// </summary>
		/// <param name="screenShot">The screenshot to read from.</param>
		/// <param name="windowstatus">Outputs the status of DutyReport or Active.</param>
		/// <returns>True if in puzzle or </returns>
		public bool InPuzzle(LockedBitmap screenShot, out PuzzleStatus windowstatus)
		{
			if (screenShot.GetPixel(OriginPoint.X, OriginPoint.Y).ToleranceEquals(InPuzzleWindowColor))
			{
				windowstatus = PuzzleStatus.Active;
				return true;
			}
			if (screenShot.GetPixel(OriginPoint.X, OriginPoint.Y).ToleranceEquals(InPuzzleWindowDrColor))
			{
				windowstatus = PuzzleStatus.DutyReport;
				return true;
			}
			
			windowstatus = PuzzleStatus.Unknown;
			return false;
		}


		public string PrintBoard(int[][] board)
		{
			var stringBuilder = new StringBuilder();

			stringBuilder.Append("***************");
			stringBuilder.Append("\n");

			for (int y = 0; y < BoardHeight; y++)
			{
				for (int x = 0; x < BoardWidth; x++)
				{
					stringBuilder.Append(
						(board[y][x] switch
						{
							(int)Tile.SAIL => "S",
							(int)Tile.ROPE => "R",
							(int)Tile.CANNON => "C",
							(int)Tile.WOOD => "W",
							(int)Tile.GOLD => "G",
							(int)Tile.UNKNOWN => "?",
							(int)Tile.EMPTY => " ",
							_ => throw new ArgumentOutOfRangeException()
						}));
				}
				stringBuilder.Append("\n");
			}
			stringBuilder.Append("**************");

			return stringBuilder.ToString();
		}

		private int IdentifyIndex(Point inputPoint)
		{
			//Zero out the point location for analysis
			var tempPoint = inputPoint.Subtract(new Point(PuzzleLocations.PieceRect.X1, PuzzleLocations.PieceRect.Y1)).Subtract(BoardOffset);

			return tempPoint.Y switch
			{
				< 100 => tempPoint.X switch
				{
					< 100 => 0,
					> 200 => 2,
					_ => 1
				},
				> 100 => tempPoint.X switch
				{
					< 100 => 3,
					> 200 => 5,
					_ => 4
				},
				_ => 99
			};
		}

		
			
		private void InitializePieces()
		{
			foreach (Pieces piece in Enum.GetValues(typeof(Pieces)))
			{
				AllPieces.Add(InitPiece(piece));

			}
		}
		public Piece InitPiece(Pieces piece)
		{
			return piece switch
			{
				Pieces.Batten => new Piece
									(
										piece.ToString(),
										new int[1][]
										{
											new int[3] { (int)Tile.SAIL, (int)Tile.WOOD, (int)Tile.SAIL }
										},
										3,
										Properties.Resources.batten.ToLockedBitmap(),
										new Point(1,0)
									),
				Pieces.Bobstay => new Piece
									(
										piece.ToString(),
										new int[3][]
										{
											new int[1] { (int)Tile.ROPE },
											new int[1] { (int)Tile.ROPE },
											new int[1] { (int)Tile.WOOD }
										},
										3,
										Properties.Resources.bobstay.ToLockedBitmap(),
										new Point(0, 1)
									),
				Pieces.Bollard => new Piece
									(
										piece.ToString(),
										new int[3][]
										{
											new int[1] { (int)Tile.ROPE },
											new int[1] { (int)Tile.WOOD },
											new int[1] { (int)Tile.WOOD }
										},
										3,
										Properties.Resources.bollard.ToLockedBitmap(),
										new Point(0, 1)
									),
				Pieces.Cannon => new Piece
									(
										piece.ToString(),
										new int[1][]
										{
											new int[3] { (int)Tile.CANNON, (int)Tile.CANNON, (int)Tile.CANNON }
										},
										3,
										Properties.Resources.cannon.ToLockedBitmap(),
										new Point(1, 0)
									),
				Pieces.Cleat => new Piece
									(
										piece.ToString(),
										new int[1][]
										{
											new int[3] { (int)Tile.CANNON, (int)Tile.ROPE, (int)Tile.CANNON }
										},
										3,
										Properties.Resources.cleat.ToLockedBitmap(),
										new Point(1, 0)
									),
				Pieces.Cringle => new Piece
									(
										piece.ToString(),
										new int[2][]
										{
											new int[2] { (int)Tile.SAIL, (int)Tile.EMPTY },
											new int[2] { (int)Tile.CANNON, (int)Tile.SAIL }
										},
										3,
										Properties.Resources.cringle.ToLockedBitmap(),
										new Point(0, 1)
									),
				Pieces.Flag => new Piece
									(
										piece.ToString(),
										new int[1][]
										{
											new int[3] { (int)Tile.WOOD, (int)Tile.SAIL, (int)Tile.SAIL }
										},
										3,
										Properties.Resources.flag.ToLockedBitmap(),
										new Point(1, 0)
									),
				Pieces.Gaff => new Piece
									(
										piece.ToString(),
										new int[2][]
										{
											new int[2] { (int)Tile.WOOD, (int)Tile.SAIL },
											new int[2] { (int)Tile.EMPTY, (int)Tile.WOOD }
										},
										3,
										Properties.Resources.gaff.ToLockedBitmap(),
										new Point(1, 0)
									),
				Pieces.Halyard => new Piece
									(
										piece.ToString(),
										new int[3][]
										{
											new int[1] { (int)Tile.ROPE },
											new int[1] { (int)Tile.WOOD },
											new int[1] { (int)Tile.SAIL }
										},
										3,
										Properties.Resources.halyard.ToLockedBitmap(),
										new Point(0, 1)
									),
				Pieces.Hatch => new Piece
									(
										piece.ToString(),
										new int[2][]
										{
											new int[2] { (int)Tile.EMPTY, (int)Tile.ROPE },
											new int[2] { (int)Tile.WOOD, (int)Tile.WOOD }
										},
										3,
										Properties.Resources.hatch.ToLockedBitmap(),
										new Point(1, 1)
									),
				Pieces.Jib => new Piece
									(
										piece.ToString(),
										new int[2][]
										{
											new int[2] { (int)Tile.SAIL, (int)Tile.EMPTY },
											new int[2] { (int)Tile.ROPE, (int)Tile.SAIL }
										},
										3,
										Properties.Resources.jib.ToLockedBitmap(),
										new Point(0, 1)
									),
				Pieces.Knee => new Piece
									(
										piece.ToString(),
										new int[2][]
										{
											new int[2] { (int)Tile.CANNON, (int)Tile.WOOD },
											new int[2] { (int)Tile.EMPTY, (int)Tile.CANNON }
										},
										3,
										Properties.Resources.knee.ToLockedBitmap(),
										new Point(1, 0)
									),
				Pieces.Pump => new Piece
									(
										piece.ToString(),
										new int[3][]
										{
											new int[1] { (int)Tile.CANNON },
											new int[1] { (int)Tile.WOOD },
											new int[1] { (int)Tile.WOOD }
										},
										3,
										Properties.Resources.pump.ToLockedBitmap(),
										new Point(0, 1)
									),
				Pieces.Rigging => new Piece
									(
										piece.ToString(),
										new int[3][]
										{
											new int[1] { (int)Tile.ROPE },
											new int[1] { (int)Tile.ROPE },
											new int[1] { (int)Tile.ROPE }
										},
										3,
										Properties.Resources.rigging.ToLockedBitmap(),
										new Point(0, 1)
									),
				Pieces.Shackle => new Piece
									(
										piece.ToString(),
										new int[1][]
										{
											new int[3] { (int)Tile.ROPE, (int)Tile.CANNON, (int)Tile.SAIL }
										},
										3,
										Properties.Resources.shackle.ToLockedBitmap(),
										new Point(1, 0)

									),
				Pieces.Thimble => new Piece
									(
										piece.ToString(),
										new int[2][]
										{
											new int[2] { (int)Tile.CANNON, (int)Tile.ROPE },
											new int[2] { (int)Tile.ROPE, (int)Tile.EMPTY }
										},
										3,
										Properties.Resources.thimble.ToLockedBitmap(),
										new Point(0, 0)
									),
				//************************ SIZE 4 PIECES ************************//
				Pieces.Ballast => new Piece
									(
										piece.ToString(),
										new int[2][]
										{
											new int[2] { (int)Tile.CANNON, (int)Tile.CANNON },
											new int[2] { (int)Tile.CANNON, (int)Tile.CANNON }
										},
										4,
										Properties.Resources.ballast.ToLockedBitmap(),
										new Point(0, 0)
									),
				Pieces.Barrel => new Piece
									(
										piece.ToString(),
										new int[2][]
										{
											new int[2] { (int)Tile.WOOD, (int)Tile.CANNON },
											new int[2] { (int)Tile.CANNON, (int)Tile.WOOD }
										},
										4,
										Properties.Resources.barrel.ToLockedBitmap(),
										new Point(0, 0)
									),
				Pieces.Block => new Piece
									(
										piece.ToString(),
										new int[3][]
										{
											new int[2] { (int)Tile.CANNON, (int)Tile.ROPE },
											new int[2] { (int)Tile.ROPE, (int)Tile.EMPTY },
											new int[2] { (int)Tile.ROPE, (int)Tile.EMPTY }
										},
										4,
										Properties.Resources.block.ToLockedBitmap(),
										new Point(0, 1)
									),
				Pieces.Boltrope => new Piece
									(
										piece.ToString(),
										new int[2][]
										{
											new int[2] { (int)Tile.SAIL, (int)Tile.ROPE },
											new int[2] { (int)Tile.ROPE, (int)Tile.SAIL }
										},
										4,
										Properties.Resources.boltrope.ToLockedBitmap(),
										new Point(0, 0)
									),
				Pieces.Boom => new Piece
									(
										piece.ToString(),
										new int[2][]
										{
											new int[2] { (int)Tile.SAIL, (int)Tile.SAIL },
											new int[2] { (int)Tile.WOOD, (int)Tile.WOOD }
										},
										4,
										Properties.Resources.boom.ToLockedBitmap(),
										new Point(0, 0)
									),
				Pieces.Mast => new Piece
									(
										piece.ToString(),
										new int[4][]
										{
											new int[1] { (int)Tile.ROPE },
											new int[1] { (int)Tile.WOOD },
											new int[1] { (int)Tile.ROPE },
											new int[1] { (int)Tile.WOOD }
										},
										4,
										Properties.Resources.mast.ToLockedBitmap(),
										new Point(0, 1)
									),
				Pieces.Rudder => new Piece
									(
										piece.ToString(),
										new int[2][]
										{
											new int[3] { (int)Tile.CANNON, (int)Tile.CANNON, (int)Tile.WOOD },
											new int[3] { (int)Tile.EMPTY, (int)Tile.EMPTY, (int)Tile.WOOD }
										},
										4,
										Properties.Resources.rudder.ToLockedBitmap(),
										new Point(1, 0)
									),
				Pieces.Sail => new Piece
									(
										piece.ToString(),
										new int[2][]
										{
											new int[2] { (int)Tile.SAIL, (int)Tile.SAIL },
											new int[2] { (int)Tile.SAIL, (int)Tile.SAIL }
										},
										4,
										Properties.Resources.sail.ToLockedBitmap(),
										new Point(0, 0)
									),
				Pieces.Shroud => new Piece
									(
										piece.ToString(),
										new int[2][]
										{
											new int[2] { (int)Tile.ROPE, (int)Tile.ROPE },
											new int[2] { (int)Tile.ROPE, (int)Tile.ROPE }
										},
										4,
										Properties.Resources.shroud.ToLockedBitmap(),
										new Point(0, 0)
									),
				Pieces.Yard => new Piece
									(
										piece.ToString(),
										new int[2][]
										{
											new int[2] { (int)Tile.WOOD, (int)Tile.WOOD },
											new int[2] { (int)Tile.SAIL, (int)Tile.SAIL }
										},
										4,
										Properties.Resources.yard.ToLockedBitmap(),
										new Point(0, 0)
									),
				//************************ SIZE 5 PIECES ************************//
				Pieces.Anchor => new Piece
									(
										piece.ToString(),
										new int[3][]
										{
											new int[3] { (int)Tile.EMPTY, (int)Tile.ROPE,(int)Tile.EMPTY },
											new int[3] { (int)Tile.EMPTY, (int)Tile.ROPE,(int)Tile.EMPTY },
											new int[3] { (int)Tile.CANNON, (int)Tile.CANNON,(int)Tile.CANNON }
										},
										5,
										Properties.Resources.anchor.ToLockedBitmap(),
										new Point(1, 1)
									),
				Pieces.Berth => new Piece
									(
										piece.ToString(),
										new int[2][]
										{
											new int[4] { (int)Tile.EMPTY, (int)Tile.EMPTY, (int)Tile.EMPTY, (int)Tile.WOOD },
											new int[4] { (int)Tile.SAIL, (int)Tile.SAIL, (int)Tile.SAIL, (int)Tile.WOOD }
										},
										5,
										Properties.Resources.berth.ToLockedBitmap(),
										new Point(2, 1)
									),
				Pieces.Bowspirit => new Piece
									(
										piece.ToString(),
										new int[2][]
										{
											new int[3] { (int)Tile.SAIL, (int)Tile.ROPE, (int)Tile.EMPTY },
											new int[3] { (int)Tile.WOOD, (int)Tile.WOOD, (int)Tile.WOOD }
										},
										5,
										Properties.Resources.bowspirit.ToLockedBitmap(),
										new Point(1, 1)
									),
				Pieces.Capstan => new Piece
									(
										piece.ToString(),
										new int[3][]
										{
											new int[3] { (int)Tile.CANNON, (int)Tile.EMPTY, (int)Tile.EMPTY },
											new int[3] { (int)Tile.ROPE, (int)Tile.ROPE, (int)Tile.ROPE },
											new int[3] { (int)Tile.CANNON, (int)Tile.EMPTY, (int)Tile.EMPTY }
										},
										5,
										Properties.Resources.capstan.ToLockedBitmap(),
										new Point(1, 1)
									),
				Pieces.Gangway => new Piece
									(
										piece.ToString(),
										new int[2][]
										{
											new int[3] { (int)Tile.ROPE, (int)Tile.EMPTY, (int)Tile.ROPE },
											new int[3] { (int)Tile.WOOD, (int)Tile.WOOD, (int)Tile.WOOD }
										},
										5,
										Properties.Resources.gangway.ToLockedBitmap(),
										new Point(1, 1)
									),
				Pieces.Helm => new Piece
									(
										piece.ToString(),
										new int[3][]
										{
											new int[3] { (int)Tile.EMPTY, (int)Tile.WOOD, (int)Tile.EMPTY },
											new int[3] { (int)Tile.WOOD, (int)Tile.CANNON, (int)Tile.WOOD },
											new int[3] { (int)Tile.EMPTY, (int)Tile.WOOD, (int)Tile.EMPTY }
										},
										5,
										Properties.Resources.helm.ToLockedBitmap(),
										new Point(1, 1)
									),
				Pieces.Lateen => new Piece
									(
										piece.ToString(),
										new int[2][]
										{
											new int[3] { (int)Tile.WOOD, (int)Tile.SAIL, (int)Tile.EMPTY },
											new int[3] { (int)Tile.WOOD, (int)Tile.SAIL, (int)Tile.SAIL }
										},
										5,
										Properties.Resources.lateen.ToLockedBitmap(),
										new Point(1, 1)
									),
				Pieces.Mooring => new Piece
									(
										piece.ToString(),
										new int[3][]
										{
											new int[3] { (int)Tile.ROPE, (int)Tile.ROPE, (int)Tile.ROPE },
											new int[3] { (int)Tile.CANNON, (int)Tile.EMPTY, (int)Tile.EMPTY },
											new int[3] { (int)Tile.CANNON, (int)Tile.EMPTY, (int)Tile.EMPTY }
										},
										5,
										Properties.Resources.mooring.ToLockedBitmap(),
										new Point(0, 0)
									),
				Pieces.Nest => new Piece
									(
										piece.ToString(),
										new int[3][]
										{
											new int[3] { (int)Tile.CANNON, (int)Tile.WOOD, (int)Tile.CANNON },
											new int[3] { (int)Tile.EMPTY, (int)Tile.ROPE, (int)Tile.EMPTY },
											new int[3] { (int)Tile.EMPTY, (int)Tile.WOOD, (int)Tile.EMPTY }
										},
										5,
										Properties.Resources.nest.ToLockedBitmap(),
										new Point(1, 1)
									),
				Pieces.Shot => new Piece
									(
										piece.ToString(),
										new int[3][]
										{
											new int[2] { (int)Tile.EMPTY, (int)Tile.CANNON },
											new int[2] { (int)Tile.CANNON, (int)Tile.SAIL },
											new int[2] { (int)Tile.SAIL, (int)Tile.CANNON }
										},
										5,
										Properties.Resources.shot.ToLockedBitmap(),
										new Point(1, 1)
									),
				_ => throw new ArgumentOutOfRangeException(nameof(piece), piece, null)
			};

		}

	}


	internal static class TileColors
	{
		public static readonly Color SAIL = Color.FromArgb(234, 218, 186);
		public static readonly Color ROPE = Color.FromArgb(154, 122, 9);
		public static readonly Color CANNON = Color.FromArgb(80, 71, 65);
		public static readonly Color WOOD = Color.FromArgb(208, 188, 131);
		public static readonly Color GOLD = Color.FromArgb(249, 241, 197);
	}

	internal static class PuzzleLocations
	{
		public static readonly RECT PieceRect = new RECT(0, 307, 300, 506);		//From BoardRef
		public static readonly Point FlagZero = new Point(-30, 201);		//From BoardRef
	}

	public enum Tile
	{
		EMPTY = 0,
		SAIL = 1,
		ROPE,
		CANNON,
		WOOD,
		GOLD,
		UNKNOWN
	}

	internal enum PuzzleStatus
	{
		DutyReport,
		Active,
		NOT,
		Unknown
	}


}
