using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ShipRight
{
	internal class Runner : IRunner
	{
		private readonly IBoardReader _boardReader;
		private readonly IMouseMovement _mouseMovement;
		private readonly IConfiguration _configuration;
		private readonly IPuzzler _puzzler;
		private readonly IAction _action;
		private CancellationTokenSource _cts;
		private readonly Stopwatch _anchorCheckStopwatch = new();

		private Thread _runningThread;
		private bool _boardFound = false;
		private int _lastFlag = 99;
		private int _failCount = 0;

		[DllImport("user32.dll")]
		static extern IntPtr GetForegroundWindow();
		private IntPtr foregroundWindowHandle;

		public Runner(IBoardReader boardReader, IMouseMovement mouseMovement, IConfiguration configuration, IPuzzler puzzler, IAction action)
		{
			_boardReader = boardReader;
			_mouseMovement = mouseMovement;
			_configuration = configuration;
			_puzzler = puzzler;
			_action = action;
		}

		public bool IsRunning { get; set; }



		private void RunLoop(CancellationToken cancellationToken)
		{
			_anchorCheckStopwatch.Start();
			
			while (IsRunning && !cancellationToken.IsCancellationRequested)
			{
				GC.Collect();
				if (!IsPpForeground()) continue;

				//cancellationToken.ThrowIfCancellationRequested();
				try
				{
					_failCount++;
					
					if (_boardReader.BoardOffset == default || _anchorCheckStopwatch.ElapsedMilliseconds >= 30000)
					{
						MainForm.SetLabel(MainForm.Labels.Status, "Getting Board Anchor", Color.Black);
						if (!_boardReader.GetAnchorPoint(_configuration.PpWindow))
						{
							MainForm.SetLabel(MainForm.Labels.Status, "Board not found", Color.Orange);
							Thread.Sleep(1000);
							if (_configuration.Automatic)
                            {
                                _action.StartStation();
                                Thread.Sleep(2000);
                                if (_failCount >= 20)
                                {
                                    _action.ClickPlayButton();
                                    _action.PlayAgain();
                                }
                            }
							continue;
						}
						MainForm.SetLabel(MainForm.Labels.Status, "Board set", Color.Black);
						_anchorCheckStopwatch.Restart();
					}

					if (_boardReader.InPuzzle(_configuration.PpWindow, out var status))
					{
						if (status == PuzzleStatus.DutyReport && _configuration.Automatic)
						{
							_action.PlayAgain();
							Thread.Sleep(1000);
							_action.ClickPlayButton();
							continue;
						}
						//MainForm.SetLabel(MainForm.Labels.Status, "Thinking", Color.Yellow);
						_anchorCheckStopwatch.Reset();
						var hasFlag = _boardReader.TryGetFlag(out var flagPos);
						if (_lastFlag == 99 && !hasFlag)
						{
							//Debug.WriteLine("Can't find flag");
							continue;
						}

						if (hasFlag)
							_lastFlag = flagPos;
						if (!hasFlag)
							flagPos = 150;

						if (!_boardReader.TryGetBoard(out var board))
						{
							//Debug.WriteLine("Could not get board");
							continue;
						}

						if (!_boardReader.TryGetPieces(out var currentPieces))
						{
							//Debug.WriteLine("Could not get pieces");
							continue;
						}

						_failCount = 0;
						//Debug.WriteLine($"Pieces: {string.Join(",", currentPieces.Select(x => x.Name))}");
						//Debug.WriteLine(_boardReader.PrintBoard(board));

						MainForm.SetLabel(MainForm.Labels.Status, "Doing Puzzle Things", Color.Green);

						/*
						var flagPos = 0;
						var currentPieces = new List<Piece>{
							_boardReader.InitPiece(Pieces.Rigging),
							_boardReader.InitPiece(Pieces.Knee),
							_boardReader.InitPiece(Pieces.Boom),
							_boardReader.InitPiece(Pieces.Yard),
							_boardReader.InitPiece(Pieces.Nest),
							_boardReader.InitPiece(Pieces.Anchor)
	
							};
						var board = new int[][]
						{
							new int[] {2,4,4,1,3},
							new int[] {2,3,4,5,5},
							new int[] {4,4,5,3,1},
							new int[] {2,1,3,4,2},
							new int[] {1,4,3,1,2}
						};*/

						
						_puzzler.Puzzle(cancellationToken, flagPos, board, currentPieces);

						Debug.WriteLine("");
						_anchorCheckStopwatch.Start();

					}

					//Always delay
					Thread.Sleep(200);
				}
				catch
				{
					_anchorCheckStopwatch.Start();
					Debug.WriteLine("Swallow me.");
					//throw;
				}
			}
		}

		public async void Run()
		{
			_cts = new CancellationTokenSource();
			IsRunning = true;
			MainForm.SetLabel(MainForm.Labels.Status, "Started", Color.Green);
			_mouseMovement.ClearAverages();
			try
			{
				await Task.Run(() => RunLoop(_cts.Token), _cts.Token);
			}
			catch (OperationCanceledException)
			{
				IsRunning = false;
				Debug.WriteLine("Canceled");
				MainForm.SetLabel(MainForm.Labels.Status, "Stopped", Color.Red);
				// Handle cancellation
			}
			finally
			{
				IsRunning = false;
				MainForm.SetLabel(MainForm.Labels.Status, "Stopped", Color.Red);
			}
		}

		public void Interrupt()
		{
			MainForm.SetLabel(MainForm.Labels.Status, "Stopping...", Color.Orange);
			_cts?.Cancel();
		}

		private bool IsPpForeground()
		{
			// Get the handle of the current foreground window
			foregroundWindowHandle = GetForegroundWindow();
			if (foregroundWindowHandle != _configuration.PpWindow)
			{
				MainForm.SetLabel(MainForm.Labels.Status, "PP not in foreground", Color.Orange);
				Thread.Sleep(1000);
				return false;
			}
			return true;
		}
	}
}
