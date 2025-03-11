using System.Collections.ObjectModel;

using Chameleon.lib.Common.Constants;
using Chameleon.lib.CommunityToolkit.MvvM;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.app.Avalonia.ViewModels;
public class PaginatorButtonViewModel {
	public int Index { get; }
	public string Text { get; }
	public bool IsEllipsis { get; }

	public PaginatorButtonViewModel(int index, bool isEllipsis = false) {
		Index = index;
		IsEllipsis = isEllipsis;
		Text = isEllipsis
				? "..."
				: (Index + 1).ToString();
	}
}
public partial class PaginatorViewModel(Action<PaginatorViewModel> ChangePageIndex, int onPageItems = Consts.PageinationPageItems)
		: ViewModelObjectBase {
	[ObservableProperty]
	private int pageCount;
	[ObservableProperty]
	private int pageIndex = -2;
	[ObservableProperty]
	private int totalCount;

	[ObservableProperty]
	private int currentIndex = -2;

	private int lastPageIndex = -3;

	public int OnPageItems { get; private set; } = onPageItems;

	public ObservableCollection<PaginatorButtonViewModel> Buttons { get; } = [];

	public int FirstVisibleElementNumber => Math.Min(Math.Abs(Skip) + 1, TotalCount);
	public int LastVisibleElementNumber => Math.Min(Math.Abs(Skip) + OnPageItems, TotalCount);

	public int Skip => PageIndex * OnPageItems;
	public bool PrevButtonIsEnabled => PageIndex > 0;
	public bool NextButtonIsEnabled => PageIndex < Buttons.Count - 1;

	partial void OnPageCountChanged(int value) {
		UpdatePaginatorButtons();
		UpdateStatus();
	}
	partial void OnPageIndexChanged(int value) {
		if (value == -1 || lastPageIndex == value )
			return;

		lastPageIndex = value;

		UpdatePaginatorButtons();
		UpdateStatus();
		ChangePageIndex(this);
	}
	partial void OnTotalCountChanged(int value) {
		if (isUpdatingButtons)
			return;

		UpdatePageCount(OnPageItems);
		UpdateStatus();
	}

	[RelayCommand]
	private void OnNextPage() {
		if (PageIndex < PageCount - 1) {
			PageIndex++;
			OnPropertyChanged(nameof(PageIndex));
		}
	}

	[RelayCommand]
	private void OnPrevPage() {
		if (PageIndex > 0) {
			PageIndex--;
			OnPropertyChanged(nameof(PageIndex));
		}
	}


	public void UpdatePageCount(int opi) {
		OnPageItems = opi;

		var pageCounts = OnPageItems == 0 ? 1 : TotalCount / OnPageItems +
				(TotalCount % OnPageItems > 0 ? 1 : 0);

		PageCount = Math.Max(1, pageCounts);
		PageIndex = 0;
		CurrentIndex = 0;
		ChangePageIndex(this);
	}

	private bool isUpdatingButtons;
	private void UpdatePaginatorButtons() {
		if (isUpdatingButtons)
			return;

		isUpdatingButtons = true;
		try {
			Buttons.Clear();

			if (PageCount <= 5) {
				for (var i = 0; i < PageCount; i++) {
					Buttons.Add(new PaginatorButtonViewModel(i));
				}
				return;
			}

			Buttons.Add(new PaginatorButtonViewModel(0));

			var start = PageIndex - 2;
			var end = PageIndex + 2;

			if (start < 1) start = 1;
			if (end > PageCount - 2) end = PageCount - 2;

			if (start > 1) {
				Buttons.Add(new PaginatorButtonViewModel(-1) {
				});
			}

			for (var i = start; i <= end; i++) {
				Buttons.Add(new PaginatorButtonViewModel(i));
			}

			if (end < PageCount - 2) {
				Buttons.Add(new PaginatorButtonViewModel(-1) {
				});
			}

			Buttons.Add(new PaginatorButtonViewModel(PageCount - 1));
		} finally {
			isUpdatingButtons = false;
		}
	}

	private void UpdateStatus() {
		OnPropertyChanged(nameof(PrevButtonIsEnabled));
		OnPropertyChanged(nameof(NextButtonIsEnabled));
		OnPropertyChanged(nameof(FirstVisibleElementNumber));
		OnPropertyChanged(nameof(LastVisibleElementNumber));
	}
}
