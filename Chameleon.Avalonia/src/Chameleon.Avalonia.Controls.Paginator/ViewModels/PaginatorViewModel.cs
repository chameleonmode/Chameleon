using Chameleon.Core.Collections.Views;
using Chameleon.Interfaces.Paginator;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Avalonia.Controls.Paginator.ViewModels;

public partial class PaginatorViewModel
    : ObservableObject
    , IPaginatorViewModel
{
    public PaginatorViewModel(int totalCount, int onPageItems = 10)
    {
        OnPageItems = onPageItems;
        TotalCount = totalCount;

    }


    [RelayCommand]
    private void OnNextPage()
    {
        PageIndex++;
    }
    [RelayCommand]
    private void OnPrevPage()
    {
        PageIndex--;
    }

    private ObservableCollectionView<PaginatorButtonViewModel> _buttons;
    public ObservableCollectionView<PaginatorButtonViewModel> Buttons
    {
        get
        {
            if (_buttons == null)
            {
                _buttons = new ObservableCollectionView<PaginatorButtonViewModel>();

                for (int i = 0; i < PageCount; i++)
                {
                    _buttons.Add(new PaginatorButtonViewModel(i));
                }

                UpdateStatus();
            }

            return _buttons;
        }
        set
        {
            SetProperty(ref _buttons, value);
        }
    }

    public bool PrevButtonIsEnabled => PageIndex > 0;
    public bool NextButtonIsEnabled => PageIndex < Buttons.Count - 1;

    private int _pageIndex;
    public int PageIndex
    {
        get { return _pageIndex; }
        set
        {
            if (SetProperty(ref _pageIndex, value))
            {
                UpdateStatus();

                ChangePageIndex?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public EventHandler ChangePageIndex { get; set; }

    private int _pageCount;
    public int PageCount
    {
        get => _pageCount;
        set
        {
            if (SetProperty(ref _pageCount, value))
            {
                UpdateButtons();
            }
        }
    }

    private void UpdateButtons()
    {
        int currentButtonsCount = Buttons.Count;

        if (currentButtonsCount < PageCount)
        {
            for (int i = currentButtonsCount; i < PageCount; i++)
            {
                _buttons.Add(new PaginatorButtonViewModel(i));
            }

            return;
        }

        if (PageIndex >= PageCount)
        {
            PageIndex = PageCount - 1;
        }

        for (int i = PageCount; i < currentButtonsCount; i++)
        {
            _buttons.Remove(_buttons[PageCount]);
        }
    }

    private void UpdatePageCount()
    {
        int pageCounts = TotalCount / OnPageItems +
            (TotalCount % OnPageItems > 0 ? 1 : 0);

        PageCount = Math.Max(1, pageCounts);
    }

    private void UpdateStatus()
    {
        OnPropertyChanged(nameof(PrevButtonIsEnabled));
        OnPropertyChanged(nameof(NextButtonIsEnabled));
        OnPropertyChanged(nameof(FirstVisibleElementNumber));
        OnPropertyChanged(nameof(LastVisibleElementNumber));
    }


    public int FirstVisibleElementNumber => Math.Min(Math.Abs(Skip) + 1, TotalCount);
    public int LastVisibleElementNumber => Math.Min(Math.Abs(Skip) + OnPageItems, TotalCount);

    private int _totalCount;
    public int TotalCount
    {
        get => _totalCount;
        set
        {
            if (SetProperty(ref _totalCount, value))
            {
                UpdatePageCount();
                UpdateStatus();
            }
        }
    }

    public int Skip => PageIndex * OnPageItems;

    public int OnPageItems { get; }
}
