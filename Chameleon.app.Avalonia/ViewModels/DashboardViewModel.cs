using System.ComponentModel;
using Chameleon.Interfaces.App.Synchronization.Events;
using CommunityToolkit.Mvvm.ComponentModel;

using CommunityToolkit.Mvvm.Input;
using Chameleon.lib.CommunityToolkit.MvvM;
using DynamicData.Binding;
using System.Reactive.Linq;
using DynamicData;
using Chameleon.app.Avalonia.Models;
using Chameleon.lib.Api.Repos;
using System.Collections.ObjectModel;
using Chameleon.lib.Common.Constants;
using Chameleon.app.Avalonia.Com.DynamicData;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.Core.Extensions;
using AutoMapper;
using System;
using System.Reactive.Subjects;
using DynamicData.Tests;

namespace Chameleon.app.Avalonia.ViewModels;
public partial class DashboardViewModel
			 : ViewModelObjectBase {
	private readonly BehaviorSubject<IComparer<UserProfileVim>> _profilesCompareObservable;
	private readonly BehaviorSubject<IComparer<FolderVim>> _foldersCompareObservable;

	[ObservableProperty]
	private bool isSyncChangesBtnVisible = true;
	[ObservableProperty]
	private Enums.ChangeComparereOption sortSelected = Enums.ChangeComparereOption.Ascending;
	[ObservableProperty]
	private Enums.ChangeComparereOption folderSortSelected = Enums.ChangeComparereOption.Ascending;

	public Enums.ChangeComparereOption[] Sorts { get; } = (Enums.ChangeComparereOption[])Enum.GetValues(typeof(Enums.ChangeComparereOption));

	public ReadOnlyObservableCollection<UserProfileVim> Profiles { get; }
	public ReadOnlyObservableCollection<FolderVim> Folders { get; }

	public bool HasNoFolderItems => Folders.Count == 0;
	public bool HasNoItems => Profiles.Count == 0;

	public DashboardViewModel() 
		: base("Dashboard")
	{
		//OnSortSelectedChanged(Enums.ChangeComparereOption.Ascending);
		_profilesCompareObservable = new BehaviorSubject<IComparer<UserProfileVim>>(Compares.UserProfileVimCompares.AscendingComparer);
		_ = UserProfilesRepo
			.Connect(i => i.isFavourite)
			.Transform(i => new UserProfileVim(i, false))
			.SortAndBind(out var list, _profilesCompareObservable)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasNoItems)); 
			});
		Profiles = list;

		//OnFolderSortSelectedChanged(Enums.ChangeComparereOption.Ascending);
		_foldersCompareObservable = new BehaviorSubject<IComparer<FolderVim>>(Compares.FolderVimCompares.AscendingComparer);
		_ = UserProfilesFolderRepo
			.Connect(i => i.isFavorite)
			.Transform(i => new FolderVim(i))
			.SortAndBind(out var flist, _foldersCompareObservable)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasNoFolderItems));
			});
		Folders = flist;

		AsyncCommandMap["SyncChanges"] = SyncChanges;
	}

	partial void OnSortSelectedChanged(Enums.ChangeComparereOption value)
	{
		_profilesCompareObservable.OnNext(value switch { 
			Enums.ChangeComparereOption.Descending => Compares.UserProfileVimCompares.DescendingComparer,
			_ => Compares.UserProfileVimCompares.AscendingComparer
		});
	}

  partial void OnFolderSortSelectedChanged(Enums.ChangeComparereOption value)
	{
		_foldersCompareObservable.OnNext(value switch {
			Enums.ChangeComparereOption.Descending => Compares.FolderVimCompares.DescendingComparer,
			_ => Compares.FolderVimCompares.AscendingComparer
		});
	}

	private async Task SyncChanges()
	{
		await AppStartup.Instance.LoadSink();
	}
}

