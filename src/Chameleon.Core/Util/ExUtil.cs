namespace Chameleon.Core.Util;

public static class ExUtil
{
    public static void TryOrCatch(Action action, Action? caught = null)
    {
        //TODO: refactu ??
        try
        {
            action();
        }
        catch
        {
            caught?.Invoke();
            //ignore
        }
    }

    public static void TryCatch(Action action, Action? caught = null)
    {
        try
        {
            action();
        }
        catch(Exception ex) 
        {
            caught?.Invoke();
            Console.WriteLine(ex.ToString());

        }
    }
}
