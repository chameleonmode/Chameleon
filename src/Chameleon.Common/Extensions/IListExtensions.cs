using System;
using System.Collections.Generic;

namespace Chameleon.Common.Extensions
{
    public static class IListExtensions
    {
        public static void AddRange<TSource>(this IList<TSource> self, IEnumerable<TSource> source)
        {
            foreach (var item in source)
            {
                self.Add(item);
            }
        }

        public static void RemoveRange<TSource>(this IList<TSource> self, IEnumerable<TSource> source)
        {
            foreach (var item in source)
            {
                self.Remove(item);
            }
        }

        public static void ReAddRange<TSource>(this IList<TSource> self, IEnumerable<TSource> source)
        {
            self.Clear();
            self.AddRange(source);
        }

        public static void Remove<TSource>(this IList<TSource> self, Func<TSource, bool> predicate)
        {
            for (var i = 0; i < self.Count; ++i)
            {
                if (predicate(self[i]))
                {
                    self.RemoveAt(i--);
                }
            }
        }

        public static void AddIfMissing<TSource>(this IList<TSource> self, TSource entity)
        {
            if (self.Contains(entity))
            {
                return;
            }
            self.Add(entity);
        }

        public static void AddNew<T>(this IList<T> self, int count)
            where T : class, new()
        {
            for (var i = 0; i < count; ++i)
            {
                self.Add(new T());
            }
        }

        public static List<T> CreateNew<T>(int count)
            where T : class, new()
        {
            var list = new List<T>(count);
            list.AddNew(count);
            return list;
        }
    }
}
