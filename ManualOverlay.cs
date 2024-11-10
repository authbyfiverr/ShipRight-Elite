using GameOverlay.Drawing;
using GameOverlay.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharpDX.Direct2D1;
using Color = GameOverlay.Drawing.Color;
using Font = GameOverlay.Drawing.Font;
using Geometry = GameOverlay.Drawing.Geometry;
using Graphics = GameOverlay.Drawing.Graphics;
using Point = System.Drawing.Point;
using Rectangle = GameOverlay.Drawing.Rectangle;
using SolidBrush = GameOverlay.Drawing.SolidBrush;

namespace ShipRight
{
	internal class ManualOverlay : IDisposable, IManualOverlay
	{
		private readonly IConfiguration _configuration;
		private readonly Graphics _graphics;
		private GraphicsWindow _window;
		public bool WindowCreated { get; set; }


		private readonly Dictionary<string, SolidBrush> _brushes;


		//private List<Circle> startCircleList = new List<Circle>();
		//private List<Circle> endCircleList = new List<Circle>();
		private Circle _startCircle = new Circle(0, 0, 0);
		private Circle _endCircle = new Circle(0, 0, 0);
		private string _movesText = "";
		private List<Line> _dragLineList = new List<Line>();
		private List<ShapePerimeter> _shapesList = new List<ShapePerimeter>();

		private readonly Dictionary<string, Font> _fonts;

		private List<SolidBrush> brushes = new List<SolidBrush>();
		//private readonly Dictionary<string, GameOverlay.Drawing.Image> _images;
		public bool redraw = false;
		private readonly Random _random = new Random(55555);
		private long _lastRandomSet;

		public ManualOverlay(IConfiguration configuration)
		{
			WindowCreated = false;
			_configuration = configuration;
			_brushes = new Dictionary<string, SolidBrush>();
			_fonts = new Dictionary<string, Font>();
			//_images = new Dictionary<string, GameOverlay.Drawing.Image>();


			_graphics = new Graphics()
			{
				MeasureFPS = true,
				PerPrimitiveAntiAliasing = true,
				TextAntiAliasing = true
			};

			if (_configuration.PpWindow != IntPtr.Zero)
			{
				CreateWindow();
				WindowCreated = true;
			}

		}

		public void CreateWindow()
		{
			if (WindowCreated) return;
			_window = new StickyWindow(_configuration.PpWindow)
			{
				FPS = 60,
				IsTopmost = true,
				IsVisible = true,
				AttachToClientArea = false
			};

			_window.DestroyGraphics += _window_DestroyGraphics;
			_window.DrawGraphics += _window_DrawGraphics;
			_window.SetupGraphics += _window_SetupGraphics;
			WindowCreated = true;
		}



		private void _window_SetupGraphics(object sender, SetupGraphicsEventArgs e)
		{
			var gfx = e.Graphics;

			/*for (int i = 0; i < 50; i++)
            {
                SolidBrush _brush = gfx.CreateSolidBrush(_random.Next(0, 256), _random.Next(0, 256), _random.Next(0, 256), 124);
                brushes.Add(_brush);
            }*/


			if (e.RecreateResources)
			{
				foreach (var pair in _brushes) pair.Value.Dispose();
				//foreach (var pair in _images) pair.Value.Dispose();
			}

			
			_brushes["black"] = gfx.CreateSolidBrush(0, 0, 0);
			_brushes["white"] = gfx.CreateSolidBrush(255, 255, 255);
			_brushes["red"] = gfx.CreateSolidBrush(255, 0, 0);
			_brushes["green"] = gfx.CreateSolidBrush(0, 200, 0);
			_brushes["blue"] = gfx.CreateSolidBrush(0, 0, 255);
			_brushes["background"] = gfx.CreateSolidBrush(0x33, 0x36, 0x3F);
			_brushes["grid"] = gfx.CreateSolidBrush(255, 255, 255, 0.2f);
			_brushes["random"] = gfx.CreateSolidBrush(0, 0, 0, 124);
			_brushes["transRed"] = gfx.CreateSolidBrush(255, 0, 0, 124);
			_brushes["startColor"] = gfx.CreateSolidBrush(200, 0, 255);
			_brushes["endColor"] = gfx.CreateSolidBrush(0, 200, 255);
			_brushes["0"] = gfx.CreateSolidBrush(0, 0, 255);
			_brushes["1"] = gfx.CreateSolidBrush(255, 0, 255);
			_brushes["2"] = gfx.CreateSolidBrush(0, 255, 255);
			_brushes["3"] = gfx.CreateSolidBrush(0, 255, 0);
			_brushes["4"] = gfx.CreateSolidBrush(255, 0, 0);
			_brushes["5"] = gfx.CreateSolidBrush(255, 255, 0);
			_brushes["0t"] = gfx.CreateSolidBrush(0, 0, 255, 100);
			_brushes["1t"] = gfx.CreateSolidBrush(255, 0, 255, 100);
			_brushes["2t"] = gfx.CreateSolidBrush(0, 255, 255, 100);
			_brushes["3t"] = gfx.CreateSolidBrush(0, 255, 0, 100);
			_brushes["4t"] = gfx.CreateSolidBrush(255, 0, 0, 100);
			_brushes["5t"] = gfx.CreateSolidBrush(255, 255, 0, 100);

			_fonts["consolas"] = gfx.CreateFont("Consolas", 14);

			//_images["dik"] = gfx.CreateImage(ExtensionMethods.ImageToByte2(Properties.Resources.celliCo));

			if (e.RecreateResources) return;

			//_gridBounds = new Rectangle(300, 200, gfx.Width - 300, gfx.Height - 200);

			/*_gridGeometry = gfx.CreateGeometry();

            for (float x = _gridBounds.Left; x <= _gridBounds.Right; x += 20)
            {
                var line = new Line(x, _gridBounds.Top, x, _gridBounds.Bottom);
                _gridGeometry.BeginFigure(line);
                _gridGeometry.EndFigure(false);
            }

            for (float y = _gridBounds.Top; y <= _gridBounds.Bottom; y += 20)
            {
                var line = new Line(_gridBounds.Left, y, _gridBounds.Right, y);
                _gridGeometry.BeginFigure(line);
                _gridGeometry.EndFigure(false);
            }

            _gridGeometry.Close();*/

		}

		private void _window_DestroyGraphics(object sender, DestroyGraphicsEventArgs e)
		{
			foreach (var pair in _brushes) pair.Value.Dispose();
			//foreach (var pair in _images) pair.Value.Dispose();
		}

		private void _window_DrawGraphics(object sender, DrawGraphicsEventArgs e)
		{
			var gfx = e.Graphics;

			gfx.ClearScene();
			int count = 0;
			int colorcount = 0;
			try
			{
				gfx.DrawCircle(_brushes["startColor"], _startCircle, 4);

				for (int i = 0; i < _dragLineList.Count; i++)
				{
					gfx.DrawLine(_brushes["endColor"], _dragLineList[i], 4);
				}

				gfx.DrawText(_fonts["consolas"], _brushes["white"], _configuration.BoardOffset.X + 15,
					_configuration.BoardOffset.Y + 308, _movesText);



				/*
				for (int i = 0; i < _pieceTileList.Count; i++)
				{
					var brush = _brushes[$"{i}"];
					var brush2 = _brushes[$"{i}t"];
					for (int j = 0; j < _pieceTileList[i].Length; j++)
					{
						gfx.OutlineFillRectangle(brush, brush2, new Rectangle(_pieceTileList[i][j].X+4, _pieceTileList[i][j].Y+4,
							_pieceTileList[i][j].X + 60 - 4, _pieceTileList[i][j].Y + 60 - 4),2);
					}
				}*/

				for (int i = 0; i < _shapesList.Count; i++)
				{
					var brush = _brushes[$"{i}"];
					var brush2 = _brushes[$"{i}t"];

					foreach (var line in _shapesList[i].EdgeLines)
					{
						gfx.DrawLine(brush, line , 4);

					}
				}
			}
			catch (Exception exception)
			{
				//Debug.WriteLine(exception);
				//throw;
			}
		}

		private SolidBrush GetRandomColor()
		{
			var brush = _brushes["random"];

			brush.Color = new Color(_random.Next(0, 256), _random.Next(0, 256), _random.Next(0, 256), 124);

			return brush;
		}


		public void Run()
		{
			_window.Create();
			_window.Join();
		}

		public void Pause()
		{
			while (!_window.IsInitialized)
				Thread.Sleep(200);
			if (_window.IsPaused) return;

			_window.IsVisible = false;
			_window.Pause();

		}

		public void UnPause()
		{
			if (_window == null) return;
			if (!_window.IsPaused) return;
			_window.Unpause();
			_window.IsVisible = true;
		}

		public void Stop()
		{
			_window.Dispose();
			_graphics.Dispose();
		}

		public void ReCreate()
		{
			_window.Recreate();
		}



		public void StartCircle(int X, int Y, float radius)
		{
			_startCircle = new Circle(X, Y, radius);
			//startCircleList.Add(startCircle);
		}

		public void DragLine(int X, int Y, int X2, int Y2)
		{
			_dragLineList.Add(new Line(X, Y, X2, Y2));
		}

		public void SetText(string text)
		{
			_movesText = text;
		}

		public void CreatePiece(List<ShapePerimeter> shapes)
		{
			_shapesList = new List<ShapePerimeter>(shapes);

		}
		public void ClearMove()
		{
			//startCircleList.Clear();
			//endCircleList.Clear();
			_startCircle = new Circle(0, 0, 0);
			_dragLineList.Clear();
			_shapesList.Clear();
		}

		


		
		

		~ManualOverlay()
		{
			Dispose(false);
		}

		#region IDisposable Support
		private bool disposedValue;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				_window.Dispose();

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}

}
