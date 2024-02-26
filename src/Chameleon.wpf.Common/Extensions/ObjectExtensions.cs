using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Chameleon.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static void InvokeOnUiThread(this object self, Action callback)
        {
            Application.Current.Dispatcher.InvokeOnUiThread(callback);
        }

        public static T InvokeOnUiThread<T>(this object self, Func<T> action)
        {
            return Application.Current.Dispatcher.InvokeOnUiThread(action);
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
            catch(Exception ex)
            {
                //TODO:
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
                //TODO:
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
}
