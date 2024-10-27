using System.Collections.ObjectModel;

using Chameleon.lib.Common.Constants;
using Chameleon.lib.CommunityToolkit.MvvM;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.app.Avalonia.ViewModels;
public class PaginatorButtonViewModel(int index) {
	public int Index { get; } = index;
	public string Text => (Index + 1).ToString();
}
public partial class PaginatorViewModel(Action<PaginatorViewModel> ChangePageIndex, int onPageItems = Consts.PageinationPageItems)
		: ViewModelObjectBase {
	[ObservableProperty]
	private int pageCount;
	[ObservableProperty]
	private int pageIndex = -1;
	[ObservableProperty]
	private int totalCount;

	public int OnPageItems { get; private set; } = onPageItems;

	public ObservableCollection<PaginatorButtonViewModel> Buttons { get; } = [];

	public int FirstVisibleElementNumber => Math.Min(Math.Abs(Skip) + 1, TotalCount);
	public int LastVisibleElementNumber => Math.Min(Math.Abs(Skip) + OnPageItems, TotalCount);

	public int Skip => PageIndex * OnPageItems;
	public bool PrevButtonIsEnabled => PageIndex > 0;
	public bool NextButtonIsEnabled => PageIndex < Buttons.Count - 1;

	partial void OnPageCountChanged(int value)
	{
		Buttons.Clear();
		for (int i = 0; i < value; i++) {
			Buttons.Add(new PaginatorButtonViewModel(i));
		}
		UpdateStatus();
	}
	partial void OnPageIndexChanged(int value)
	{
		UpdateStatus();
		ChangePageIndex(this);
	}
	partial void OnTotalCountChanged(int value)
	{
		UpdatePageCount(OnPageItems);
		UpdateStatus();
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

	public void UpdatePageCount(int opi)
	{
		OnPageItems = opi;

		var pageCounts = OnPageItems == 0 ? 1 : TotalCount / OnPageItems +
				(TotalCount % OnPageItems > 0 ? 1 : 0);

		PageCount = Math.Max(1, pageCounts);
		PageIndex = 0;
		ChangePageIndex(this);
	}

	private void UpdateStatus()
	{
		OnPropertyChanged(nameof(PrevButtonIsEnabled));
		OnPropertyChanged(nameof(NextButtonIsEnabled));
		OnPropertyChanged(nameof(FirstVisibleElementNumber));
		OnPropertyChanged(nameof(LastVisibleElementNumber));
	}
}
