using System.Collections.ObjectModel;
using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.client.UI.UserControls.ViewModels;

public class PaginatorButtonViewModel(int index, bool isEllipsis = false) {
	public int Index { get; } = index;
	public bool IsEllipsis { get; } = isEllipsis;
	public string Text { get; } =
	isEllipsis ? "..." : (index + 1).ToString();
}

public partial class PaginatorViewModel(Action<PaginatorViewModel> changePageIndex, int onPageItems = 9)
: ViewModelObjectBase {
	private bool _isUpdatingButtons;
	[ObservableProperty]
	private int pageCount;

	[ObservableProperty]
	private int pageIndex;

	[ObservableProperty]
	private int totalCount;

	[ObservableProperty]
	private int currentIndex;

	public int OnPageItems { get; private set; } = onPageItems;

	public ObservableCollection<PaginatorButtonViewModel> Buttons { get; } = [];

	public int Skip => PageIndex * OnPageItems;

	public int FirstVisibleElementNumber => Math.Min(Skip + 1, TotalCount);

	public int LastVisibleElementNumber => Math.Min(Skip + OnPageItems, TotalCount);

	public bool PrevButtonIsEnabled => PageIndex > 0;
	public bool NextButtonIsEnabled => PageIndex < PageCount - 1;

	partial void OnPageCountChanged(int value) {
		UpdatePaginatorButtons();
		UpdateStatus();
	}

	partial void OnPageIndexChanged(int value) {
		UpdatePaginatorButtons();
		UpdateStatus();
		GoToPageIndex();
	}

	partial void OnTotalCountChanged(int value) {
		UpdatePageCount(OnPageItems);
		UpdateStatus();
	}

	[RelayCommand]
	private void OnNextPage() {
		if (PageIndex < PageCount - 1)
			PageIndex++;
	}

	[RelayCommand]
	private void OnPrevPage() {
		if (PageIndex > 0)
			PageIndex--;
	}

	public void UpdatePageCount(int opi) {
		OnPageItems = opi;

		var pageCounts = (OnPageItems == 0)
				? 1
				: (TotalCount / OnPageItems) + ((TotalCount % OnPageItems) > 0 ? 1 : 0);

		PageCount = Math.Max(1, pageCounts);

		PageIndex = 0;
		CurrentIndex = 0;

		GoToPageIndex();
	}

	private void GoToPageIndex() {
		CurrentIndex = PageIndex + 1;
		changePageIndex(this);
	}

	private void UpdatePaginatorButtons() {
		if (_isUpdatingButtons)
			return;

		_isUpdatingButtons = true;
		try {
			Buttons.Clear();

			if (PageCount <= 5) {
				for (var i = 0; i < PageCount; i++) {
					Buttons.Add(new PaginatorButtonViewModel(i));
				}
				return;
			}

			if (PageIndex <= 2) {
				Buttons.Add(new PaginatorButtonViewModel(0));
				Buttons.Add(new PaginatorButtonViewModel(1));
				Buttons.Add(new PaginatorButtonViewModel(2));

				Buttons.Add(new PaginatorButtonViewModel(-1, isEllipsis: true));

				Buttons.Add(new PaginatorButtonViewModel(PageCount - 1));
				return;
			}

			if (PageIndex >= PageCount - 3) {
				Buttons.Add(new PaginatorButtonViewModel(0));

				Buttons.Add(new PaginatorButtonViewModel(-1, isEllipsis: true));

				Buttons.Add(new PaginatorButtonViewModel(PageCount - 3));
				Buttons.Add(new PaginatorButtonViewModel(PageCount - 2));
				Buttons.Add(new PaginatorButtonViewModel(PageCount - 1));
				return;
			}

			Buttons.Add(new PaginatorButtonViewModel(0));
			Buttons.Add(new PaginatorButtonViewModel(-1, isEllipsis: true));
			Buttons.Add(new PaginatorButtonViewModel(PageIndex));
			Buttons.Add(new PaginatorButtonViewModel(-1, isEllipsis: true));
			Buttons.Add(new PaginatorButtonViewModel(PageCount - 1));
		} finally {
			_isUpdatingButtons = false;
		}
	}

	public void UpdateStatus() {
		OnPropertyChanged(nameof(PrevButtonIsEnabled));
		OnPropertyChanged(nameof(NextButtonIsEnabled));
		OnPropertyChanged(nameof(FirstVisibleElementNumber));
		OnPropertyChanged(nameof(LastVisibleElementNumber));
	}
}
