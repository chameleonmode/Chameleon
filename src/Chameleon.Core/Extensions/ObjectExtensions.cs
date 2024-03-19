using System.Reflection;

namespace Chameleon.Core.Extensions;

public static class ObjectExtensions
{
    public static void InvokeOnUiThread(this object self, Action callback)
    {
        throw new NotImplementedException();
        //Application.Current.Dispatcher.Invoke(callback);
    }

    public static T InvokeOnUiThread<T>(this object self, Func<T> action)
    {
        throw new NotImplementedException();
        // return Application.Current.Dispatcher.Invoke(action);
    }
    public static void TryCatchIgnore(this object self, Action action)
    {
        //TODO: refactu ??
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
