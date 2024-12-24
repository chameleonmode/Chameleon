using System.Reflection;
using Avalonia.Collections;

namespace Chameleon.Avalonia.Common.Collections;

public class AvList<TDestination> : AvaloniaList<TDestination> 
{
    public void AddMapped<TSource>(IEnumerable<TSource> collection, Func<TSource, TDestination> mapper)
    {
        foreach (TSource item in collection)
        {
            var destination = mapper(item);
            Add(destination);
        }
    }

    public void RemoveMapped<TSource>(IEnumerable<TSource> collection, Func<TSource, TDestination> mapper)
    {
        foreach (TSource item in collection)
        {
            var destination = mapper(item);
            Remove(destination);
        }
    }

    public void UpdateMapped<TSource>(IEnumerable<TSource> collection, Func<TSource, TDestination> mapper, Func<TDestination, TSource, bool> contains)
    {
        var itemsToRemove = this.Where(destItem => !collection.Any(srcItem => contains(destItem, srcItem)));
        RemoveAll(itemsToRemove);

        var itemsToAdd = collection.Where(i => !this.Any(x => contains(x, i))).Select(mapper);
        AddRange(itemsToAdd);
    }
       //foreach (TSource item in collection)
       //{
       //    if (!this.Any(x => contains(x, item)))
       //    {
       //        var destination = mapper(item); 
       //        Add(destination);
       //    }
       //}
    
       //// Remove items not in sourceCollection
       //var itemsToRemove = this.Where(destItem => !collection.Any(srcItem => contains(destItem, srcItem))).ToList();
       //foreach (var itemToRemove in itemsToRemove)
       //{
       //    Remove(itemToRemove);
       //}
    //}
}
