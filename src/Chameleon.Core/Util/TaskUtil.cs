namespace Chameleon.Core.Util;
public static class TaskUtil
{
    public static async Task<bool> AwaitFor(Func<bool> contition, int count = 5, int milleseconds = 250)
    {
        for (int i = 0; i < count; i++)
        {
            if (contition.Invoke())
                break;

            await Task.Delay(milleseconds);
        }

        return contition.Invoke();
    }

    public static async Task<T?> TryAwaitFor<T>(Func<T?> contition, int count = 5, int milleseconds = 250)
    {
        for (int i = 0; i < count; i++)
        {
            try
            {
                return contition.Invoke();
            }
            catch { await Task.Delay(milleseconds); }
        }

        return default;
    }
}
