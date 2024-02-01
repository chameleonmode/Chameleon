using Chameleon.Interfaces.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Chameleon.Domain.Entities
{
    public class ObservableEntities<TEntity, TPrimaryKey>
        : IList<TEntity>
        , INotifyCollectionChanged
        where TEntity : IEntity<TPrimaryKey>
    {
        private ObservableCollection<TEntity> Items { get; }

        public ObservableEntities()
        {
            Items = new ObservableCollection<TEntity>();
            Items.CollectionChanged += Observable_CollectionChanged;
        }


        private void Observable_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                OnResetItems((ObservableCollection<TEntity>)sender, e);
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                OnAddNewItems(e);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                OnRemoveOldItems(e);
            }
            CollectionChanged?.Invoke(this, e);
        }

        private void OnResetItems(
            ObservableCollection<TEntity> collection, 
            NotifyCollectionChangedEventArgs e
            )
        {
            if (e.NewItems != null || e.OldItems != null)
            {
                throw new NotImplementedException();
            }

            foreach (TEntity entity in collection)
            {
                if (Equals(entity.Id, default(TPrimaryKey)))
                {
                    OnAdd(entity);
                }
            }
        }

        private void OnAddNewItems(NotifyCollectionChangedEventArgs e)
        {
            foreach (TEntity entity in e.NewItems)
            {
                OnAdd(entity);
            }
        }

        protected virtual void OnAdd(TEntity entity)
        {
        }

        private void OnRemoveOldItems(NotifyCollectionChangedEventArgs e)
        {
            foreach (TEntity entity in e.OldItems)
            {
                OnRemove(entity);
            }
        }

        protected virtual void OnRemove(TEntity entity)
        {
        }

        public int Count => Items.Count;
        public bool IsReadOnly => false;

        public TEntity this[int index]
        {
            get => Items[index];
            set
            {
                RemoveAt(index);
                Insert(index, value);
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public IEnumerator<TEntity> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public int IndexOf(TEntity item)
        {
            return Items.IndexOf(item);
        }

        public void Insert(int index, TEntity item)
        {
            Items.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            Items.RemoveAt(index);
        }

        public void Add(TEntity item)
        {
            Items.Add(item);
        }

        public void Clear()
        {
            Items.Clear();
        }

        public bool Contains(TEntity item)
        {
            return Items.Contains(item);
        }

        public void CopyTo(TEntity[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        public bool Remove(TEntity item)
        {
            return Items.Remove(item);
        }

        public void Add(params TEntity[] entities)
        {
            //Items.AddRange(entities);
            foreach (var item in entities)
                Items.Add(item);
        }
    }

    public class ObservableEntities<TEntity> 
        : ObservableEntities<TEntity, int>
        where TEntity : IEntity<int>
    {
    }
}
