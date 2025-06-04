using Chameleon.client.Libs.MvvM;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Util;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Chameleon.client.Features.Projects.Profiles.Identity.ViewModels;

public abstract partial class ProfileSectionViewModel<TDto, TViewModel> : ViewModelObjectBase
		where TDto : UP, new()
		where TViewModel : MappableViewModelBase<TDto> {
	protected readonly BehaviorSubject<Func<UP, bool>> filter;
	protected readonly ReadOnlyObservableCollection<TViewModel> items;
	protected readonly UserProfileViewModel? userProfile;

	[ObservableProperty] private bool isLoading;

	[ObservableProperty] private bool isNotLoading;

	public ReadOnlyObservableCollection<TViewModel> Items => items;
	
	public bool HasNoItems => Items?.Count == 0;

	public virtual Func<UP, bool> FilterPredicate => p => p.ProfileId == userProfile?.Id;

	protected abstract UPRepo<TDto> SourceRepository { get; }

	protected abstract TViewModel CreateViewModel(TDto dto);

	protected virtual TDto CreateDto => new() { ProfileId = userProfile?.Id };

	protected ProfileSectionViewModel(UserProfileViewModel userProfile) {
		this.userProfile = userProfile;

		filter = new BehaviorSubject<Func<UP, bool>>(FilterPredicate);
		IsLoading = true;

		_ = SourceRepository.Connect().Filter(filter)
		.Transform(CreateViewModel)
		.Bind(out items)
		.Subscribe((i) => {
			IsLoading = false;
			OnPropertyChanged(nameof(HasNoItems));
		});

		_ = this.WhenValueChanged(x => x.IsLoading)
		.DistinctUntilChanged()
		.Subscribe(isLoad => IsNotLoading = !isLoad);
		
		AsyncCommandMap["AddItem"] = AddItem;
	}

	public virtual void UpdateFilter() {
		filter.OnNext(FilterPredicate);
	}

	public virtual async Task AddItem() {
		if (IsLoading || IsBusy || Items.Any(IsNewItem)) return;
		try {
			IsLoading = true;
			var newItem = await InitializeNewItem(CreateDto);
			OnPropertyChanged(nameof(HasNoItems));
		} finally {
			IsLoading = false;
		}
	}

	protected virtual Task<TDto> InitializeNewItem(TDto dto) {
		return SourceRepository.Initialize(dto);
	}

	protected virtual bool IsNewItem(TViewModel item) {
		// Default implementation assumes item has an Id property that is 0 for new items
		var id = item.GetType().GetProperty("Id")?.GetValue(item) as int?;
		return id != null && (int)id == 0;
	}

	public virtual async Task SaveItem(TViewModel item) {
		IsLoading = true;
		try {
			_ = item.IsValidationValid();
			var dto = item.ToDto();

			if (dto != null) {
				if (IsNewItem(item)) {
					_ = await UPAdditionalDataRepo.DeleteFromCache(SourceRepository, dto);
				}
				await SaveItemToRepository(item);
			}
		} finally {
			IsLoading = false;
		}
	}

	protected virtual Task SaveItemToRepository(TViewModel item) {
		return UPAdditionalDataRepo.Save(SourceRepository, item.ToDto()).RunInBackground();
	}


	[RelayCommand]
	public virtual async Task DeleteItem(TViewModel item) {
		IsLoading = true;
		try {
			var dto = item.ToDto();

			_ = IsNewItem(item)
					? await UPAdditionalDataRepo.DeleteFromCache(SourceRepository, dto)
					: await UPAdditionalDataRepo.Delete(SourceRepository, dto).RunInBackgroundWithResult();

			OnPropertyChanged(nameof(HasNoItems));
		} finally {
			IsLoading = false;
		}
	}

	public virtual void ValidateAll() {
		foreach (var item in Items) {
			_ = item.IsValidationValid();
		}
	}

	public virtual async Task SaveAll() {
		IsLoading = true;
		try {
			var saveTasks = Items.Select(SaveItem).ToArray();
			await Task.WhenAll(saveTasks);

			await SourceRepository.Load();

		} finally {
			IsLoading = false;
		}
	}
}