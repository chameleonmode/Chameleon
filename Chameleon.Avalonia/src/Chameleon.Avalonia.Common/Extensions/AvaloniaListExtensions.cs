using Avalonia.Collections;

namespace Chameleon.Avalonia.Common.Extensions;
public static class AvaloniaListExtensions
{
    public static async void AddNewRangeAsync<T>(this AvaloniaList<T> sels, Task<IEnumerable<T>> values)
    {
        sels.Clear();
        sels.AddRange(await values);
    }
}
