using Chameleon.Interfaces.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;

namespace Chameleon.Core.Extensions;

public static class ObservableCollectionExtension
{
    public static void AddRange<T>(this ObservableCollection<T> self, IEnumerable<T> enumerable)
    {
        if (enumerable == null)
        {
            return;
        }

        var items = enumerable as List<T> ?? enumerable.ToList();
        if (self == null || items.Count == 0)
        {
            return;
        }

        var type = self.GetType();

        var bindingFlags = BindingFlags.Instance
            | BindingFlags.InvokeMethod
            | BindingFlags.NonPublic
            ;

        type.InvokeMember("CheckReentrancy",
            bindingFlags,
            null, self, null
            );

        var itemsProp = type.BaseType.GetProperty("Items",
            BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance
            );

        var privateItems = itemsProp.GetValue(self) as IList<T>;
        foreach (var item in items)
        {
            privateItems.Add(item);
        }

        type.InvokeMember("OnPropertyChanged", bindingFlags, null,
          self, new object[]
          {
                  new PropertyChangedEventArgs("Count")
          });

        type.InvokeMember("OnPropertyChanged", bindingFlags, null,
          self, new object[]
          {
                  new PropertyChangedEventArgs("Item[]")
          });

        type.InvokeMember("OnCollectionChanged", bindingFlags, null,
          self, new object[]
          {
                  new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)
          });
    }

    public static Task AddRangeAsync<T>(
        this ObservableCollection<T> self,
        Func<IEnumerable<T>> getItems,
        IDispatcherService dispatcher)
    {
        return dispatcher.InvokeOnUiThreadAsync(getItems,
            items => self.AddRange(items)
            );
    }

    public static void ReAddRange<T>(this ObservableCollection<T> self, IEnumerable<T> items)
    {
        self.Clear();
        self.AddRange(items);
    }

    public static void RemoveRange<T>(this ObservableCollection<T> self, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            self.Remove(item);
        }
    }

    public static Task ReAddRangeAsync<T>(this ObservableCollection<T> self, Func<IEnumerable<T>> items,
        IDispatcherService dispatcher)
    {
        self.Clear();
        return self.AddRangeAsync(items,dispatcher);
    }
}
