using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace ShipRight
{
	internal class TimerHelper
	{
		private readonly Timer _timer;
		private readonly object _syncLock = new object();
		private bool _isCompleted;

		public TimerHelper(int interval)
		{
			_timer = new Timer(interval);
			_timer.Elapsed += OnElapsed;
			_timer.AutoReset = false;
		}

		private void OnElapsed(object sender, ElapsedEventArgs e)
		{
			lock (_syncLock)
			{
				_isCompleted = true;
				Monitor.Pulse(_syncLock);
			}
		}

		public void Start()
		{
			_isCompleted = false;
			_timer.Start();
		}

		public void WaitForCompletion()
		{
			lock (_syncLock)
			{
				if (!_isCompleted)
				{
					Monitor.Wait(_syncLock);
				}
			}
		}
	}
}
