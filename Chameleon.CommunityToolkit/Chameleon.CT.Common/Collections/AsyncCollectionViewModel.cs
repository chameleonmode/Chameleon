using System.Collections.ObjectModel;
using Chameleon.Core.Extensions;
using Chameleon.CT.Common.Base;

namespace Chameleon.CT.Common.Collections;

public class AsyncCollectionViewModel<T> : ObservableObjectBase
     where T : class
{
    private readonly Func<IEnumerable<T>> _getItems;
    public AsyncCollectionViewModel(Func<IEnumerable<T>> getItems, bool isVisible = false)
    {
        _getItems = getItems;
        _isVisible = isVisible;
    }

    private ObservableCollection<T> _items;
    public ObservableCollection<T> Items => _items;

    private T _selectedItem;
    public T SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (_selectedItem != value)
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        private set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }
    }

    public void Add(T item)
    {
        _items.Add(item);
    }

    public void Remove(T item)
    {
        var currentIndex = _items.IndexOf(item);
        if (currentIndex != -1)
        {
            _items.RemoveAt(currentIndex);
            SelectedItem = _items.ElementAtOrDefault(currentIndex);
        }
    }

    public void Clear()
    {
        if (_items == null)
        {
            return;
        }

        _items?.Clear();
        SelectedItem = null;
    }

    public Task Load()
    {
        if (_items == null)
        {
           return AddItemsAsync();
        }
        return Task.CompletedTask;
    }

    public override Task InitAsync()
    {
        return Load();
    }

    public Task Reload()
    {
        Clear();
        _isBinded = false;
       return AddItemsAsync();
    }

    private async Task AddItemsAsync()
    {
        if (_items == null)
        {
            _items = [];
        }

        IsLoading = true;
        var items = await Task.Run(_getItems);
        Items.AddRange(items);
        EnsureBinded();
        IsLoading = false;
        //_items.AddRangeAsync(_getItems, DispatcherService).ContinueWith(t =>
        //{
        //    EnsureBinded();
        //    IsLoading = false;
        //});
    }

    private bool _isVisible;
    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            if (SetProperty(ref _isVisible, value))
            {
                EnsureBinded();
            }
        }
    }

    private bool _isBinded;
    private void EnsureBinded()
    {
        if (_isBinded || !_isVisible)
        {
            return;
        }
        _isBinded = true;
        OnItemsBinded();
    }

    private void OnItemsBinded()
    {
        RaiseItemsChanged();
        SelectFirstItem();
        Binded?.Invoke(this, new EventArgs());
    }

    private void SelectFirstItem()
    {
        if (_items.Count > 0)
        {
            SelectedItem = _items[0];
        }
    }

    private void RaiseItemsChanged()
    {
        OnPropertyChanged(nameof(Items));
    }

    public event EventHandler Binded;
}
