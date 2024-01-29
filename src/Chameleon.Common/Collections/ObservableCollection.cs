using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Chameleon.Common.Collections
{
    public class ObservableCollection<TSource, TDestination> : ObservableCollection<TDestination>
    {
        private readonly Func<TSource, TDestination> _mapper;
        private readonly Dictionary<TSource, TDestination> _map 
            = new Dictionary<TSource, TDestination>();

        public ObservableCollection(
            IEnumerable<TSource> collection, 
            Func<TSource, TDestination> mapper,
            bool trackOnlyNewItems = false
            )
        {
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            _mapper = mapper;
            if (!trackOnlyNewItems)
            {
                AddRange(collection);
            }

            if (collection is INotifyCollectionChanged source)
            {
                source.CollectionChanged += Source_CollectionChanged;
            }
        }

        private void Source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                OnResetItems(e);
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                OnAddNewItems(e);
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                OnRemoveOldItems(e);
                return;
            }
        }

        private void OnResetItems(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null || e.OldItems != null)
            {
                throw new NotImplementedException();
            }

            Items.Clear();
            AddRange(this);
        }

        private void OnAddNewItems(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null)
            {
                return;
            }
            AddRange(e.NewItems);
        }

        private void OnRemoveOldItems(NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems == null)
            {
                return;
            }

            foreach (TSource item in e.OldItems)
            {
                if (_map.TryGetValue(item, out var destination))
                {
                    Remove(destination);
                }
            }
        }

        private void AddRange(IEnumerable range)
        {
            var addedItems = new List<TDestination>();
            foreach (TSource item in range)
            {
                var destination = _mapper(item);
                _map[item] = destination;
                Items.Add(destination);
                addedItems.Add(destination);
            }

            if (addedItems.Count == 0)
            {
                return;
            }

            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add, addedItems
                ));
        }
    }
}
