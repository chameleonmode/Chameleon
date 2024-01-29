using System;
using System.Windows.Threading;

// https://weblog.west-wind.com/posts/2017/jul/02/debouncing-and-throttling-dispatcher-events
namespace Chameleon.Common
{
    public class DebounceDispatcher
    {
        private readonly object _syncRoot = new object();

        private DispatcherTimer _timer;
        private DateTime _timerStarted = DateTime.UtcNow.AddYears(-1);

        public void Debounce(
            int interval,
            Action action,
            DispatcherPriority priority = DispatcherPriority.ApplicationIdle,
            Dispatcher dispatcher = null
            )
        {
            Debounce<object>(interval, _ => action(), null, priority, dispatcher);
        }

        public void Debounce<T>(
            int interval,
            Action<T> action,
            T param = default(T),
            DispatcherPriority priority = DispatcherPriority.ApplicationIdle,
            Dispatcher dispatcher = null
            )
        {
            lock (_syncRoot)
            {
                Stop();

                _timer = new DispatcherTimer(
                    TimeSpan.FromMilliseconds(interval), 
                    priority, 
                    (s, e) => OnTimer(action, param), 
                    dispatcher ?? Dispatcher.CurrentDispatcher
                    );

                _timer.Start();
            }
        }

        public void Throttle(int interval, Action action,
            DispatcherPriority priority = DispatcherPriority.ApplicationIdle,
            Dispatcher dispatcher = null)
        {
            Throttle<object>(interval, _ => action(), null, priority, dispatcher);
        }

        public void Throttle<T>(int interval, Action<T> action,
            T param = default(T),
            DispatcherPriority priority = DispatcherPriority.ApplicationIdle,
            Dispatcher dispatcher = null)
        {
            lock(_syncRoot)
            {
                // kill pending timer and pending ticks
                Stop();

                var curTime = DateTime.UtcNow;

                // if timeout is not up yet - adjust timeout to fire 
                // with potentially new Action parameters           
                if (curTime.Subtract(_timerStarted).TotalMilliseconds < interval)
                {
                    interval -= (int)curTime.Subtract(_timerStarted).TotalMilliseconds;
                }

                _timer = new DispatcherTimer(
                    TimeSpan.FromMilliseconds(interval), 
                    priority, (s, e) => OnTimer(action, param), 
                    dispatcher ?? Dispatcher.CurrentDispatcher
                    );

                _timer.Start();

                _timerStarted = curTime;
            }
        }

        private void OnTimer<T>(Action<T> action, T param)
        {
            lock (_syncRoot)
            {
                if (!Stop())
                {
                    return;
                }
            }
            action.Invoke(param);
        }

        private bool Stop()
        {
            if (_timer == null)
            {
                return false;
            }

            _timer.Stop();
            _timer = null;
            return true;
        }
    }
}

