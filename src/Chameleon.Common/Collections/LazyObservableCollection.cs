using System;
using System.Collections.Generic;

namespace Chameleon.Common.Collections
{
    public class LazyObservableCollection<TSource, TDestination> 
        : ObservableCollection<TSource, TDestination>
    {
        public LazyObservableCollection(
            ICollection<TSource> collection,
            Func<TSource, TDestination> mapper
            ) : base(collection, mapper, true)
        {
        }
    }

    public class LazyObservableCollection<TSource>
        : LazyObservableCollection<TSource, TSource>
    {
        public LazyObservableCollection(
            ICollection<TSource> collection
            ) : base(collection, _ => _)
        {
        }
    }
}
