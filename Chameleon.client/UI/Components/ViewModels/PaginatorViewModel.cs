using System.Collections.ObjectModel;
using Chameleon.client.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.client.UI.Components.ViewModels;

public record PaginatorButtonViewModel(int Index, bool IsEllipsis = false) {
	public string Text { get; } = IsEllipsis ? "..." : (Index + 1).ToString();
}

public partial class PaginatorViewModel : ViewModelObjectBase {
	private readonly Action<PaginatorViewModel> changePageIndex;
	[ObservableProperty] int pageCount;
	[ObservableProperty] int totalCount;
	[ObservableProperty] int currentIndex;
	[ObservableProperty] int pageIndex = -1; // -1 means no page is selected
	[ObservableProperty] int onPageItems = 9;

	public ObservableCollection<PaginatorButtonViewModel> Buttons { get; } = [];

	public int Skip => PageIndex * OnPageItems;
	public int FirstVisibleElementNumber => Math.Min(Skip + 1, TotalCount);
	public int LastVisibleElementNumber => Math.Min(Skip + OnPageItems, TotalCount);
	public bool PrevButtonIsEnabled => PageIndex > 0;
	public bool NextButtonIsEnabled => PageIndex < PageCount - 1;

	public PaginatorViewModel(Action<PaginatorViewModel> changePageIndex) {
		this.changePageIndex = changePageIndex;
		CommandMap["Previous"] = () => PageIndex--;
		CommandMap["Next"] = () => PageIndex++;
	}

	partial void OnPageCountChanged(int value) => UpdateAll();
	partial void OnPageIndexChanged(int value) => UpdateAll();
	partial void OnTotalCountChanged(int value) => UpdateStatus();

	public void UpdatePageCount(int opi) {
		OnPageItems = opi;
		PageCount = Math.Max(1, OnPageItems == 0 ? 1 : (TotalCount + OnPageItems - 1) / OnPageItems);
		PageIndex = 0;
		CurrentIndex = 0;
		GoToPageIndex();
	}

	private void GoToPageIndex() {
		changePageIndex(this);
		CurrentIndex = PageIndex + 1;
	}

	private void UpdateAll() {
		UpdatePaginatorButtons();
		UpdateStatus();
		GoToPageIndex();
	}

	private void UpdatePaginatorButtons() {
		Buttons.Clear();

		if (PageCount <= 5) {
			AddButtonRange(0, PageCount);
			return;
		}

		var ellipsis = new PaginatorButtonViewModel(-1, true);

		switch (PageIndex) {
			case <= 2:
				AddButtonRange(0, 3);
				Buttons.Add(ellipsis);
				Buttons.Add(new PaginatorButtonViewModel(PageCount - 1));
				break;
			case var pi when pi >= PageCount - 3:
				Buttons.Add(new PaginatorButtonViewModel(0));
				Buttons.Add(ellipsis);
				AddButtonRange(PageCount - 3, 3);
				break;
			default:
				Buttons.Add(new PaginatorButtonViewModel(0));
				Buttons.Add(ellipsis);
				Buttons.Add(new PaginatorButtonViewModel(PageIndex));
				Buttons.Add(ellipsis);
				Buttons.Add(new PaginatorButtonViewModel(PageCount - 1));
				break;
		}
	}

	private void AddButtonRange(int start, int count) {
		for (var i = 0; i < count; i++) {
			Buttons.Add(new PaginatorButtonViewModel(start + i));
		}
	}

	public void UpdateStatus() {
		OnPropertyChanged(nameof(PrevButtonIsEnabled));
		OnPropertyChanged(nameof(NextButtonIsEnabled));
		OnPropertyChanged(nameof(FirstVisibleElementNumber));
		OnPropertyChanged(nameof(LastVisibleElementNumber));
	}
}
