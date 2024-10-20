using System.Collections.ObjectModel;

using Chameleon.lib.Common.Constants;
using Chameleon.lib.CommunityToolkit.MvvM;

using CommunityToolkit.Mvvm.Input;

namespace Chameleon.app.Avalonia.ViewModels;
public class PaginatorButtonViewModel(int index) {
	public int Index { get; } = index;
	public string Text => (Index + 1).ToString();
}
public partial class PaginatorViewModel
		: ViewModelObjectBase {
	public PaginatorViewModel(int totalCount, int onPageItems = Consts.PageinationPageItems)
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

	private ObservableCollection<PaginatorButtonViewModel> _buttons;
	public ObservableCollection<PaginatorButtonViewModel> Buttons {
		get {
			if (_buttons == null) {
				_buttons = [];

				for (int i = 0; i < PageCount; i++) {
					_buttons.Add(new PaginatorButtonViewModel(i));
				}

				UpdateStatus();
			}

			return _buttons;
		}
		set {
			SetProperty(ref _buttons, value);
		}
	}

	public bool PrevButtonIsEnabled => PageIndex > 0;
	public bool NextButtonIsEnabled => PageIndex < Buttons.Count - 1;

	private int _pageIndex;
	public int PageIndex {
		get { return _pageIndex; }
		set {
			if (SetProperty(ref _pageIndex, value)) {
				UpdateStatus();

				ChangePageIndex?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	public EventHandler ChangePageIndex { get; set; }

	private int _pageCount;
	public int PageCount {
		get => _pageCount;
		set {
			if (SetProperty(ref _pageCount, value)) {
				UpdateButtons();
			}
		}
	}

	private void UpdateButtons()
	{
		var currentButtonsCount = Buttons.Count;

		if (currentButtonsCount < PageCount) {
			for (var i = currentButtonsCount; i < PageCount; i++) {
				_buttons.Add(new PaginatorButtonViewModel(i));
			}

			return;
		}

		if (PageIndex >= PageCount) {
			PageIndex = PageCount - 1;
		}

		for (var i = PageCount; i < currentButtonsCount; i++) {
			_buttons.Remove(_buttons[PageCount]);
		}
	}

	private void UpdatePageCount()
	{
		var pageCounts = TotalCount / OnPageItems +
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
	public int TotalCount {
		get => _totalCount;
		set {
			if (SetProperty(ref _totalCount, value)) {
				UpdatePageCount();
				UpdateStatus();
			}
		}
	}

	public int Skip => PageIndex * OnPageItems;

	public int OnPageItems { get; }
}
