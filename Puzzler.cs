using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameOverlay.Drawing;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using static ShipRight.Heuristic;
using Newtonsoft.Json.Linq;
using SharpDX.DirectWrite;

namespace ShipRight
{
	internal class Puzzler : IPuzzler
	{
		private readonly IConfiguration _configuration;
		private readonly IBoardReader _boardReader;
		private readonly IScreenshotService _screenshotService;
		private readonly IAction _action;
		private readonly IManualOverlay _manualOverlay;
		private readonly IMouseMovement _mouseMovement;
		private readonly Random _random;


		private Solver _solver;
		private Solution bestSolution;
		private Queue<Swap> _swaps;
		private List<Piece> _usedPieces;
		private Status _status { get; set; }
		private List<Piece> _lastPieces;
		

		private Point _lastPoint;

		private bool _finishing;
		private int _solveFlagPos;
		private int _lastFlag;
		private int _currentChain;
		private List<int> _pieceValues = new List<int>();
		private int _currentTotal;

		private CancellationToken _cancellationToken { get; set; }



		public Puzzler(IConfiguration configuration, IBoardReader boardReader, IScreenshotService screenshotService, IAction action, IManualOverlay manualOverlay, IMouseMovement mouseMovement)
		{
			_configuration = configuration;
			_boardReader = boardReader;
			_screenshotService = screenshotService;
			_action = action;
			_manualOverlay = manualOverlay;
			_mouseMovement = mouseMovement;
			_status = Status.NewSolve;
			_currentChain = default;
			_random = new Random();
		}

		public void Reset()
		{
			_solver = default;
			bestSolution = default;
			_swaps = default;
			_usedPieces = default;
			_status = default;
			_solveFlagPos = default;
			_lastFlag = default;
			_currentChain = default;
			_pieceValues = new List<int>();
			_lastPieces = new List<Piece>();
			_finishing = false;

			MainForm.SetLabel(MainForm.Labels.Flag, "0");
			MainForm.SetLabel(MainForm.Labels.Chain, "0");
			MainForm.SetLabel(MainForm.Labels.SolveScore, "0");
			MainForm.SetLabel(MainForm.Labels.AverageScore, "0");
			MainForm.SetLabel(MainForm.Labels.TotalScore, "0");
			MainForm.SetLabel(MainForm.Labels.BonusScore, "0");
			_manualOverlay.ClearMove();
			_manualOverlay.SetText("");
			MainForm.SetLabel(MainForm.Labels.Status, "Game info reset");
		}

		public void Puzzle(CancellationToken cancellationToken, int flagPos, int[][] gameBoard, List<Piece> currentPieces)
		{
			_cancellationToken = cancellationToken;
			try
			{
				_finishing = flagPos > 19;
				if (_configuration.ForceScan)
				{
					ReScan();
					
					return;
				}
				if (_finishing && !_configuration.Automatic)
					MainForm.SetLabel(MainForm.Labels.Status, "Checking for move");

				if (_finishing && !_boardReader.TryGetFlag(out var tmpFlag))
					_status = Status.Complete;

				if (_status == Status.NewSolve)
				{
					_solveFlagPos = flagPos;
					MainForm.SetLabel(MainForm.Labels.Status, "Solving: PAUSE", Color.Red);
					if (_configuration.Automatic)
						_mouseMovement.PauseGame();
					if (Solve(gameBoard, currentPieces, flagPos))
					{
						_currentChain = 0;
						_lastPieces = new List<Piece>(currentPieces);
						MainForm.SetLabel(MainForm.Labels.Chain, $"{_currentChain}");
						Debug.WriteLine("Getting swaps");
						MainForm.SetLabel(MainForm.Labels.Status, "Getting Swaps: PAUSE", Color.Red);

						if (GetSwaps(gameBoard, bestSolution.Board))
						{
							Debug.WriteLine("Drawing");
							Debug.WriteLine($"Total swaps: {_swaps.Count}");
							_status = Status.Arrange;
							//MainForm.SetLabel(MainForm.Labels.Status, "Ready: UNPAUSE", Color.DarkGreen);
						}
						else
						{
							MainForm.SetLabel(MainForm.Labels.Status, "No swaps! Move a piece", Color.Red);
							//TODO Handle no swaps
						}
					}

					if (_configuration.Automatic)
						_mouseMovement.UnPauseGame();
				}

				if (_status is Status.Arrange or Status.WaitArrange)
				{
					MainForm.SetLabel(MainForm.Labels.Status, "Make board swaps", Color.DarkGreen);
					if (!_configuration.Automatic)
						Draw(cancellationToken);
					else
					{
						Arrange(cancellationToken);
					}
				}
				
				if (_status == Status.WaitingForMove)
				{
					var evaluate = false;
					//Evaluate current pieces and possible placement pieces in relation to flagpos
					
					if (!MadeMove(currentPieces)) return;

					Debug.WriteLine("Move made");
					MainForm.SetLabel(MainForm.Labels.Status, "Move made. Scanning..");

					if (flagPos is >= 18 and < 20)
					{
						Thread.Sleep(1000);
						_boardReader.TryGetFlag(out var curFlagPos);
						if (curFlagPos == 19)
						{
							evaluate = true;
						}
					}

					_usedPieces.Clear();
					_manualOverlay.ClearMove();
					if (Solve(gameBoard, currentPieces, flagPos))
					{
						MainForm.SetLabel(MainForm.Labels.Chain, $"{_currentChain}");
						_lastPieces = new List<Piece>(currentPieces);
						_status = Status.Placing;

						if (evaluate)
						{
							if (_currentChain == 1)
							{
								//Continue through the solve
							}
							else if (_currentChain < _configuration.FinishChain)
							{
								_status = Status.NewSolve;
								Debug.WriteLine($"Resolving - Current Chain: {_currentChain}");
								return;
							}
							/*
							Debug.WriteLine("Evaluating)");
							var newList = new List<Piece>(_usedPieces).OrderBy(x => x.Size).ToList();
							var newValuesList = new List<int>(_pieceValues);
							var tempChain = _currentChain;
							var score = 0;

							foreach (var piece in newList)
							{
								tempChain++;
								score += (piece.Size - 2) * tempChain;
								newValuesList.Add(score);
							}

							var average = score / _pieceValues.Count;
							if (average < _pieceValues.Average())
							{
								_status = Status.NewSolve;
								return;
							}
							*/
						}
					}
					else
					{
						_currentChain = 0;
						_manualOverlay.ClearMove();
						if (_finishing)
						{
							_status = Status.Complete;
						}
						else
						{
							_status = Status.NewSolve;
							MainForm.SetLabel(MainForm.Labels.Chain, $"{_currentChain}");
						}
					}
				}
				if (_status == Status.Placing)
				{
					MainForm.SetLabel(MainForm.Labels.Status, "Place a move");
					Debug.WriteLine("Place a move");

					if (_configuration.Automatic)
						PlacePiece();
					else
						DrawPieces();

					_status = Status.WaitingForMove;
					_lastFlag = flagPos;


				}
				if (_status == Status.Abandon)
				{
					Abandon();
				}

				if (_status == Status.Complete)
				{
					while (_boardReader.TryGetFlag(out var _) && !_cancellationToken.IsCancellationRequested)
					{
						MainForm.SetLabel(MainForm.Labels.Status, _finishing ? "Game Over" : "ABANDON", Color.Red);
						Thread.Sleep(1000);
					}

					Thread.Sleep(_random.Next(2431, 5617));

					if (_configuration.Automatic)
					{
                        MainForm.SetLabel(MainForm.Labels.LastGame, $"{_pieceValues.Sum(),0:##.00}");
                        Reset();
                        _action.PlayAgain();

					}
					else
					{
						while (_action.CheckForPlayAgain() && !_cancellationToken.IsCancellationRequested)
						{
							MainForm.SetLabel(MainForm.Labels.Status, _finishing ? "Game Over" : "ABANDON", Color.Red);
							Thread.Sleep(500);
						}
                        MainForm.SetLabel(MainForm.Labels.LastGame, $"{_pieceValues.Sum(),0:##.00}");
                        Reset();
                    }
                }
			}
			catch (Exception e)
			{
				Debug.WriteLine(e);
				//throw;
			}
		}


		private void Abandon()
		{
			MainForm.SetLabel(MainForm.Labels.Status, _finishing ? "Game Over" : "ABANDON", Color.Red);

			Thread.Sleep(_random.Next(204, 467));

			if (_configuration.Automatic)
			{
				MainForm.SetLabel(MainForm.Labels.Status, "Exiting Puzzle", Color.Red);
				_action.ExitPuzzle();
				Thread.Sleep(_random.Next(2205, 6241));
				MainForm.SetLabel(MainForm.Labels.Status, "Finding Station", Color.Red);
				_action.StartStation();
				
			}
			else
			{
				while (_boardReader.TryGetFlag(out var _) && !_cancellationToken.IsCancellationRequested)
				{
					Thread.Sleep(500);
					Debug.WriteLine("Waiting");
				}
				
			}
			Reset();

		}




		private bool MadeMove(List<Piece> currentPieces)
		{
			var usedPiece = _lastPieces.Except(currentPieces).FirstOrDefault();
			if (usedPiece == null) return false;
			_currentChain++;

			Debug.WriteLine($"Current chain {_currentChain}");
			MainForm.SetLabel(MainForm.Labels.Chain, $"{_currentChain}");

			_pieceValues.Add((usedPiece.Size - 2) * _currentChain);
			Debug.WriteLine($"Placed {usedPiece.Name} of size {usedPiece.Size} for combo value of {(usedPiece.Size - 2) * _currentChain} ");

			MainForm.SetLabel(MainForm.Labels.TotalScore, $"{_pieceValues.Sum(),0:##.00}");
			MainForm.SetLabel(MainForm.Labels.AverageScore, $"{_pieceValues.Average(),0:##.00}");

			return true;
		}

		private bool Solve(int[][] gameBoard, List<Piece> currentPieces, int flagPos)
		{
			var tileCounts = CountTiles(gameBoard);

			bool[][] placedPieces = new bool[5][];
			for (int i = 0; i < 5; i++)
			{
				placedPieces[i] = new bool[5];
			}
			currentPieces = currentPieces.OrderByDescending(p => p.Size).ToList();

			if (_status == Status.NewSolve)
			{
				var solveBoard = InitSolveBoard(gameBoard);
				_solver = new Solver();
				bestSolution = _solver.InitSolve(solveBoard, currentPieces, 0, tileCounts, placedPieces, _boardReader.AllPieces);
				_usedPieces = bestSolution.Pieces.Where( x => x.Used && currentPieces.Any(o => o.Name == x.Name)).ToList();

				if (bestSolution == null)
				{
					Debug.WriteLine("No solution");
					return false;
				}

				if (flagPos == 0 && _configuration.RejectBoards && _solver.GetBestScore() < (double)_configuration.RejectScore)
				{
					_status = Status.Abandon;
					return false;
				}
			}
			else if (_status == Status.WaitingForMove)
			{
				SolverNoMoves solverNoMoves = new SolverNoMoves();
				solverNoMoves.InitSolve(gameBoard, currentPieces, tileCounts);
				try
				{
					_usedPieces = solverNoMoves.GetUsedPieces().Where(x => x.Used).ToList();
					Debug.WriteLine($"new used Pieces {string.Join(", ", _usedPieces.Select(x => x.Name))}");
					return true;
				}
				catch (System.ArgumentNullException e) { }
				{
					Debug.WriteLine("No more options. Solve again");
					
					return false;
				}
			}

			Debug.WriteLine("Solve done");
			
			return true;
		}

		private bool GetSwaps(int[][] currentBoard, int[][] goalBoard)
		{
			Stopwatch sw = Stopwatch.StartNew();
			var swaps = ArrangeTiles.FindTileSwaps(currentBoard, goalBoard, _cancellationToken);
			sw.Stop();
			Debug.WriteLine("*******");
			Debug.WriteLine($"Time to get swaps: {sw.Elapsed.TotalSeconds}");
            if (swaps == null)
            {
                Debug.WriteLine("No swaps!!!!");
                //Move some pieces
                return false;
            }
            Debug.WriteLine($"Swaps: {swaps.Count}");
			Debug.WriteLine("");

			
			//Debug.WriteLine($"Swaps: {swaps.Count}");
			_swaps = new Queue<Swap>(swaps);
			return true;
		}

		private void Draw(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested && ( _status == Status.Arrange || _status == Status.WaitArrange))
			{
				if (_configuration.ForceScan)
				{
					ReScan();
				}

				if (!_boardReader.TryGetBoard(out var curBoard))
				{
					Thread.Sleep(50);
					continue;
				}

				if (_status == Status.WaitArrange)
				{
					var thisSwap = _swaps.Peek();
					if (Extensions.AreJaggedArraysEqual(curBoard, thisSwap.StartingBoard))
					{
						Debug.WriteLine("Success swap.");
						_swaps.Dequeue();
						_manualOverlay.ClearMove();
						_status = Status.Arrange;
					}
				}

				if (_status == Status.Arrange)
				{
					if (!_swaps.TryPeek(out var currentSwap))
					{
						//Done
						Debug.WriteLine($"{_solver.GetOutput()}");
						_manualOverlay.ClearMove();
						_manualOverlay.SetText("");						
						_status = Status.Placing;
						return;
					}

					CombineSwapsToChain(cancellationToken, currentSwap);
				}
			}
		}

		private void Arrange(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested && (_status == Status.Arrange || _status == Status.WaitArrange))
			{
				if (_configuration.ForceScan)
				{
					ReScan();
				}

				if (!_boardReader.TryGetBoard(out var curBoard))
				{
					Thread.Sleep(50);
					continue;
				}
				if (_status == Status.WaitArrange)
				{
					var thisSwap = _swaps.Peek();
					if (Extensions.AreJaggedArraysEqual(curBoard, thisSwap.StartingBoard))
					{
						Debug.WriteLine("Success swap.");
						_swaps.Dequeue();
						_status = Status.Arrange;
						_manualOverlay.ClearMove();
						_manualOverlay.SetText("");
					}
				}

				if (_status == Status.Arrange)
				{
					if (!_swaps.TryPeek(out var currentSwap))
					{
						//Done
						Debug.WriteLine($"{_solver.GetOutput()}");
						_manualOverlay.ClearMove();
						_manualOverlay.SetText("");
						_status = Status.Placing;
						return;
					}

					if (!_configuration.Automatic)
						CombineSwapsToChain(cancellationToken, currentSwap);
					else
					{
						_manualOverlay.SetText($"{_swaps.Count} swaps left");
						var tile1 = _boardReader.GetGridCenterPoint(currentSwap.Tile1.X, currentSwap.Tile1.Y);
						var tile2 = _boardReader.GetGridCenterPoint(currentSwap.Tile2.X, currentSwap.Tile2.Y);
						_mouseMovement.HumanMoveMouseMovement(tile1.Randomize(16));
						Thread.Sleep(_random.Next(15, 50));
						_mouseMovement.LeftClick();
						Thread.Sleep(_random.Next(15, 50));
						_mouseMovement.HumanMoveMouseMovement(tile2.Randomize(16));
						Thread.Sleep(_random.Next(15, 50));
						_mouseMovement.LeftClick();
						Thread.Sleep(_random.Next(50, 150));
						_status = Status.WaitArrange;
					}
				}
			}
		}

		private void DrawPieces()
		{
			var shapesToDraw = new List<ShapePerimeter>();
			foreach (var piece in _usedPieces)
			{
				var perimeterShape = new ShapePerimeter(piece.BoardPos.DeepClone());

				for (int i = 0; i < piece.BoardPos.Length; i++)
				{
					for (int j = 0; j < piece.BoardPos[i].Length; j++)
					{
						if (piece.BoardPos[j][i] != 0)
						{
							var pixelOriginPoint = _boardReader.GetGridPoint(i, j);
							perimeterShape.TilesArray[j][i] = new PerimeterTile(i, j, pixelOriginPoint);
							ShapeCreator.IsPerimeterCell(perimeterShape.Shape, i, j, perimeterShape.TilesArray[j][i]);
						}
					}
				}

				ShapeCreator.BuildEdges(perimeterShape);
				shapesToDraw.Add(perimeterShape.Clone());

			}
			_manualOverlay.CreatePiece(shapesToDraw);
		}

		private void PlacePiece()
		{
			//TODO Select piece with best overlap
			var bestPiece = _usedPieces.MinBy(x => x.Size);
			var placementPoint = new Point(999,999);

			for (int i = 0; i < bestPiece.BoardPos.Length; i++)
			{
				for (int j = 0; j < bestPiece.BoardPos[i].Length; j++)
				{
					if (bestPiece.BoardPos[j][i] == 99)
					{
						placementPoint = new Point(i, j);
						break;
					}
				}
			}

			if (placementPoint.X != 999)
			{
				var placementPixel = _boardReader.GetGridCenterPoint(placementPoint.X, placementPoint.Y).Randomize(10);
				var pickupPixel = bestPiece.IndexPoint.Randomize(20);

				_mouseMovement.HumanMoveMouseMovement(pickupPixel);
				_mouseMovement.LeftClick();
				_mouseMovement.HumanMoveMouseMovement(placementPixel);
				_mouseMovement.LeftClick();

				if (_finishing)
				{
					_mouseMovement.PauseGame();
					Thread.Sleep(_random.Next(1300,1800));
					_mouseMovement.UnPauseGame();
					Thread.Sleep(_random.Next(30,130));
				}


				//DrawLine(pickupPixel, placementPixel);
				Debug.WriteLine("Move drawn");
			}
			else
			{
				Debug.WriteLine("Error. no pieces?");
			}
		}

		private void CombineSwapsToChain(CancellationToken cancellationToken, Swap currentSwap)
		{
			var pointList = new List<Point>();
			var pointUsageCount = new Dictionary<Point, int>();
			for (int i = -1; i < _swaps.Count - 1; i++)
			{
				var curSwap = i >= 0 ? _swaps.ElementAt(i) : currentSwap;
				var nextSwap = _swaps.ElementAt(i + 1);

				// Get the starting and ending points of curSwap and nextSwap
				var curStart = new Point(curSwap.Tile1.X, curSwap.Tile1.Y);
				var curEnd = new Point(curSwap.Tile2.X, curSwap.Tile2.Y);
				var nextStart = new Point(nextSwap.Tile1.X, nextSwap.Tile1.Y);
				var nextEnd = new Point(nextSwap.Tile2.X, nextSwap.Tile2.Y);

				Point p1 = Point.Empty, p2 = Point.Empty;

				if (curEnd == nextStart)
				{
					p1 = _boardReader.GetGridCenterPoint(curEnd.X, curEnd.Y);
					p2 = _boardReader.GetGridCenterPoint(nextEnd.X, nextEnd.Y);
				}
				else if (curEnd == nextEnd)
				{
					p1 = _boardReader.GetGridCenterPoint(curEnd.X, curEnd.Y);
					p2 = _boardReader.GetGridCenterPoint(nextStart.X, nextStart.Y);
				}
				
				else if (curStart == nextEnd)
				{
					p1 = _boardReader.GetGridCenterPoint(curStart.X, curStart.Y);
					p2 = _boardReader.GetGridCenterPoint(nextStart.X, nextStart.Y);
				}
				else if (curStart == nextStart)
				{
					p1 = _boardReader.GetGridCenterPoint(curStart.X, curStart.Y);
					p2 = _boardReader.GetGridCenterPoint(nextEnd.X, nextEnd.Y);
				}
				else
				{
					break;
				}



				if (!pointUsageCount.ContainsKey(p1)) pointUsageCount[p1] = 0;
				if (!pointUsageCount.ContainsKey(p2)) pointUsageCount[p2] = 0;

				// Check if any point is used more than twice and break the loop
				if (pointUsageCount[p1] >= 2 || pointUsageCount[p2] >= 2)
				{
					break;
				}


				pointList.Add(p1);
				pointList.Add(p2);

				pointUsageCount[p1]++;
				pointUsageCount[p2]++;
				


			}

			// Find start and end points (those with usage count 1)
			Point startPoint = Point.Empty;
			Point endPoint = Point.Empty;

			foreach (var entry in pointUsageCount)
			{
				if (entry.Value == 1)
				{
					if (startPoint.IsEmpty && pointUsageCount.Count > 2)
						startPoint = entry.Key;
					else
					{
						endPoint = entry.Key;
						break;
					}
				}
			}

			if (startPoint.IsEmpty || endPoint.IsEmpty)
			{
				startPoint = _boardReader.GetGridCenterPoint(currentSwap.Tile1.X, currentSwap.Tile1.Y);
				endPoint = _boardReader.GetGridCenterPoint(currentSwap.Tile2.X, currentSwap.Tile2.Y);

				if (endPoint == _lastPoint)
				{
					startPoint = endPoint;
					endPoint = _boardReader.GetGridCenterPoint(currentSwap.Tile1.X, currentSwap.Tile1.Y);
				}


				DrawLine(startPoint, endPoint);
				Debug.WriteLine($"Drag {currentSwap.Tile1Type} to {currentSwap.Tile2Type}");
				Debug.WriteLine($"");
			}
			else
			{
				
				for (int i = 0; i < pointList.Count - 1; i++)
				{
					Debug.WriteLine($"From {pointList[i]} to {pointList[i + 1]}");
					
						DrawLine(pointList[i], pointList[i + 1]);
						endPoint = pointList[i];
					
				}
			}

			_lastPoint = endPoint;

			_manualOverlay.StartCircle(startPoint.X, startPoint.Y, 20);
			_status = Status.WaitArrange;
			_manualOverlay.SetText($"{_swaps.Count} swaps left");

		}

		private void ReScan()
		{
			_status = Status.NewSolve;
			_usedPieces.Clear();
			_manualOverlay.ClearMove();
			_configuration.ForceScan = false;
		}
		
		private int[][] InitSolveBoard(int[][] board)
		{
			var newBoard = new int[5][];
			for (int i = 0; i < 5; i++)
			{
				newBoard[i] = new int[5];
				for (int j = 0; j < 5; j++)
				{
					newBoard[i][j] = 0; // initialize with 0
				}
			}

			for (int y = 0; y < board.Length; y++)
			{
				for (int x = 0; x < board[y].Length; x++)
				{
					if ((Tile)board[y][x] == Tile.GOLD)
					{
						newBoard[y][x] = (int)Tile.GOLD;
					}
				}
			}
			return newBoard;
		}

		private static Dictionary<Tile, int> CountTiles(int[][] board)
		{
			Dictionary<Tile, int> tileCounts = new Dictionary<Tile, int>
			{
				{ Tile.ROPE, 0 },
				{ Tile.SAIL, 0 },
				{ Tile.CANNON, 0 },
				{ Tile.WOOD, 0 },
				{ Tile.GOLD, 0 }
			};

			for (int y = 0; y < board.Length; y++)
			{
				for (int x = 0; x < board[y].Length; x++)
				{
					Tile currentTile = (Tile)board[y][x];
					tileCounts[currentTile]++;
				}
			}

			return tileCounts;
		}

		private void DrawLine(Point p1, Point p2)
		{
			_manualOverlay.DragLine(p1.X, p1.Y, p2.X, p2.Y);
		}

	}

	public enum Status
	{
		NewSolve,
		Arrange,
		WaitArrange,
		Placing,
		WaitingForMove,
		Finishing,
		Abandon,
		Evaluate,
		Complete
	}

	public enum Direction
	{
		UP,
		DOWN,
		LEFT,
		RIGHT,
		UPLEFT,
		UPRIGHT,
		DOWNLEFT,
		DOWNRIGHT,
		UNKNOWN
	}
}
