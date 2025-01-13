using Avalonia.Collections;

namespace Chameleon.app.Avalonia.Extensions;

public static class AvaloniaListExt {
	public static void UpdateMapped<TSource, TDestination>(this AvaloniaList<TDestination> cur, IEnumerable<TSource> collection, Func<TSource, TDestination> mapper, Func<TDestination, TSource, bool> contains)
	{
		var itemsToRemove = cur.Where(destItem => !collection.Any(srcItem => contains(destItem, srcItem)));
		cur.RemoveAll(itemsToRemove);

		var itemsToAdd = collection.Where(i => !cur.Any(x => contains(x, i))).Select(mapper);
		cur.AddRange(itemsToAdd);
	}

	public static void AddMapped<TSource, TDestination>(this AvaloniaList<TDestination> cur, IEnumerable<TSource> collection, Func<TSource, TDestination> mapper)
	{
		foreach (var item in collection) {
			var destination = mapper(item);
			cur.Add(destination);
		}
	}
}