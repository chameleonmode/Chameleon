using Chameleon.Avalonia.Prism.Module.Base;
using System.Collections.ObjectModel;
using Chameleon.Core.Extensions;

namespace Chameleon.Avalonia.Prism.Module.Collections;

public class AsyncCollectionViewModel<T> : ViewModelBase
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
                RaisePropertyChanged(nameof(SelectedItem));
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
                RaisePropertyChanged(nameof(IsLoading));
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

    public void Load()
    {
        if (_items == null)
        {
            AddItemsAsync();
        }
    }

    public void Reload()
    {
        Clear();
        _isBinded = false;
        AddItemsAsync();
    }

    private void AddItemsAsync()
    {
        if (_items == null)
        {
            _items = new ObservableCollection<T>();
        }

        IsLoading = true;
        _items.AddRangeAsync(_getItems,DispatcherService).ContinueWith(t =>
        {
            EnsureBinded();
            IsLoading = false;
        });
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
        RaisePropertyChanged(nameof(Items));
    }

    public event EventHandler Binded;
}
