using System.Reflection;

namespace Chameleon.Core.Extensions;

public static class ObjectExtensions
{
    public static void InvokeOnUiThread(this object self, Action callback)
    {
        //Application.Current.Dispatcher.Invoke(callback);
    }

    public static T InvokeOnUiThread<T>(this object self, Func<T> action)
    {
        throw new NotImplementedException();
        // return Application.Current.Dispatcher.Invoke(action);
    }

    public static void InvokeOnUiThread(this object self, EventHandler handler, EventArgs args = null)
    {
        self.InvokeOnUiThread(() =>
        {
            handler?.Invoke(self, args ?? new EventArgs());
        });
    }

    public static Task InvokeOnUiThreadAsync<T>(this object self, Func<T> action, Action<T> handler = null, Action @finally = null)
    {
        return Task.Run(() =>
        {
            try
            {
                if (!TryExecute(action, out var result))
                {
                    return;
                }

                if (handler != null)
                {
                    self.InvokeOnUiThread(() =>
                    {
                        handler(result);
                    });
                }
            }
            finally
            {
                @finally?.Invoke();
            }
        });
    }

    public static Task InvokeOnUiThreadAsync(this object self, Action action, Action<bool> handler = null, Action @finally = null)
    {
        return Task.Run(() =>
        {
            try
            {
                var success = TryExecute(action);

                if (handler != null)
                {
                    self.InvokeOnUiThread(() =>
                    {
                        handler(success);
                    });
                }
            }
            finally
            {
                @finally?.Invoke();
            }
        });
    }

    private static Action Noop = () => { };
    public static Task InvokeOnUiThreadAsync(this object self, Action action)
    {
        return Task.Run(() =>
        {
            var success = TryExecute(action);
            self.InvokeOnUiThread(Noop);
        });
    }

    public static async void InvokeAsync(this object self, Func<Task> action)
    {
        await _InvokeAsync(action);
    }

    private static async Task _InvokeAsync(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            //ExceptionHandler.ShowException(ex);
        }
    }

    private static bool TryExecute<T>(Func<T> action, out T result)
    {
        try
        {
            result = action();
            return true;
        }
        catch (Exception ex)
        {
            //ExceptionHandler.ShowException(ex);
        }
        result = default(T);
        return false;
    }

    private static bool TryExecute(Action action)
    {
        return TryExecute(() =>
        {
            action();
            return true;
        }, out var result);
    }

    public static void TryCatchIgnore(this object self, Action action)
    {
        try
        {
            action();
        }
        catch
        {
            //ignore
        }
    }

    public static T1 CopyFrom<T1, T2>(this T1 destObject, T2 srcObject)
        where T1 : class
        where T2 : class
    {
        PropertyInfo[] srcFields = srcObject
            .GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

        PropertyInfo[] destFields = destObject
            .GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);

        foreach (var property in srcFields)
        {
            var dest = destFields
                .FirstOrDefault(x => x.Name == property.Name);

            if (dest != null && dest.CanWrite)
            {
                dest.SetValue(destObject, property.GetValue(srcObject, null), null);
            }
        }

        return destObject;
    }
}
