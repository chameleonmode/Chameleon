using Avalonia.Threading;

namespace Chameleon.Avalonia.Common.Util;

public class DebounceDispatcher
{
    private readonly object _syncRoot = new();

    private DispatcherTimer? _timer;

    public void Debounce(
        int interval,
        Action action,
        DispatcherPriority? priority = null)
    {
        priority ??= DispatcherPriority.ApplicationIdle;
        Debounce<object>(interval, _ => action(), null, priority);
    }

    public void Debounce<T>(
        int interval,
        Action<T> action,
        T? param = default,
        DispatcherPriority? priority = null
        )
    {
        lock (_syncRoot)
        {
            
            Stop();

            _timer = new DispatcherTimer(
                TimeSpan.FromMilliseconds(interval),
                priority ??= DispatcherPriority.ApplicationIdle,
                (s, e) => OnTimer(action, param)
                );

            _timer.Start();
        }
    }

    private void OnTimer<T>(Action<T?> action, T? param)
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

        _timer?.Stop();
        _timer = null;
        return true;
    }
}
