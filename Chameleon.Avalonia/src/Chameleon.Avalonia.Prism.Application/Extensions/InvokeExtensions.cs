using Avalonia.Threading;
using Chameleon.Core.Extensions;

namespace Chameleon.Avalonia.Prism.Application.Extensions;

public static class InvokeExtensions
{
   //public static void InvokeOnUiThread(this object self, Action callback)
   //{
   //    Dispatcher.UIThread.Post(callback);
   //    //Avalonia.Application.
   //    //Application.Current.Dispatcher.Invoke(callback);
   //}
   //
   //public static T InvokeOnUiThread<T>(this object self, Func<T> action)
   //{
   //    return Dispatcher.UIThread.Invoke(action);
   //    //throw new NotImplementedException();
   //    // return Application.Current.Dispatcher.Invoke(action);
   //}

   //public static void InvokeOnUiThread(this object self, EventHandler handler, EventArgs args = null)
   //{
   //    self.InvokeOnUiThread(() =>
   //    {
   //        handler?.Invoke(self, args ?? new EventArgs());
   //    });
   //}
  //
  //public static Task InvokeOnUiThreadAsync<T>(this object self, Func<T> action, Action<T> handler = null, Action @finally = null)
  //{
  //    return Task.Run(() =>
  //    {
  //        try
  //        {
  //            if (!ObjectExtensions.TryExecute(action, out var result))
  //            {
  //                return;
  //            }
  //
  //            if (handler != null)
  //            {
  //                self.InvokeOnUiThread(() =>
  //                {
  //                    handler(result);
  //                });
  //            }
  //        }
  //        finally
  //        {
  //            @finally?.Invoke();
  //        }
  //    });
  //}
  //
  //public static Task InvokeOnUiThreadAsync(this object self, Action action, Action<bool> handler = null, Action @finally = null)
  //{
  //    return Task.Run(() =>
  //    {
  //        try
  //        {
  //            var success = ObjectExtensions.TryExecute(action);
  //
  //            if (handler != null)
  //            {
  //                self.InvokeOnUiThread(() =>
  //                {
  //                    handler(success);
  //                });
  //            }
  //        }
  //        finally
  //        {
  //            @finally?.Invoke();
  //        }
  //    });
  //}
  //
  //private static Action Noop = () => { };
  //public static Task InvokeOnUiThreadAsync(this object self, Action action)
  //{
  //    return Task.Run(() =>
  //    {
  //        var success = ObjectExtensions.TryExecute(action);
  //        self.InvokeOnUiThread(Noop);
  //    });
  //}
}
