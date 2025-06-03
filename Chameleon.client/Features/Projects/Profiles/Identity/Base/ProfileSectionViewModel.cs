using Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity.ViewModels;
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

namespace Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity.Base;

public abstract partial class ProfileSectionViewModel<TDto, TViewModel> : ViewModelObjectBase
		where TDto : UP, new()
		where TViewModel : ViewModelObjectBase {
	protected readonly BehaviorSubject<Func<UP, bool>> filter;
	protected readonly ReadOnlyObservableCollection<TViewModel> items;
	protected readonly UserProfileViewModel? userProfile;
	protected readonly string collectionPropertyName;
	protected readonly string hasItemsPropertyName;

	[ObservableProperty] private bool isLoading;

	[ObservableProperty] private bool isNotLoading;

	public ReadOnlyObservableCollection<TViewModel> Items => items;

	public bool HasItems => Items?.Count > 0;

	// Properties for legacy API compatibility - derived classes can use these or override them
	public virtual ReadOnlyObservableCollection<TViewModel> Persons => items;
	public virtual bool HasPersons => HasItems;
	public virtual ReadOnlyObservableCollection<TViewModel> Businesses => items;
	public virtual bool HasBusiness => HasItems;
	public virtual ReadOnlyObservableCollection<TViewModel> Addresses => items;
	public virtual bool HasAddresses => HasItems;
	public virtual ReadOnlyObservableCollection<TViewModel> Logins => items;
	public virtual bool HasLogins => HasItems;

	public virtual Func<UP, bool> FilterPredicate => p => p.ProfileId == userProfile?.Id;

	protected abstract UPRepo<TDto> SourceRepository { get; }

	protected abstract TViewModel CreateViewModel(TDto dto);

	protected virtual TDto CreateDto() {
		return new TDto { ProfileId = userProfile?.Id };
	}

	protected ProfileSectionViewModel(
			UserProfileViewModel? userProfile,
			string collectionPropertyName,
			string hasItemsPropertyName) {
		this.userProfile = userProfile;
		this.collectionPropertyName = collectionPropertyName;
		this.hasItemsPropertyName = hasItemsPropertyName;

		filter = new BehaviorSubject<Func<UP, bool>>(FilterPredicate);
		IsLoading = true;

		_ = SourceRepository
				.Connect()
				.Filter(filter)
				.Transform(CreateViewModel)
				.Bind(out items)
				.Subscribe((i) => {
					IsLoading = false;
					OnPropertyChanged(hasItemsPropertyName);
					OnPropertyChanged(nameof(HasItems));
				});

		_ = this.WhenValueChanged(x => x.IsLoading)
			.DistinctUntilChanged()
			.Subscribe(isLoad => IsNotLoading = !isLoad);
	}

	public virtual void UpdateFilter() {
		IsLoading = true;
		filter.OnNext(FilterPredicate);
		IsLoading = false;
	}

	[RelayCommand]
	public virtual async Task AddItem() {
		if (IsLoading || IsBusy)
			return;

		if (Items.Any(IsNewItem)) {
			return;
		}

		IsLoading = true;
		try {
			var newItem = await InitializeNewItem(CreateDto());
			OnPropertyChanged(hasItemsPropertyName);
			OnPropertyChanged(nameof(HasItems));
		} finally {
			IsLoading = false;
		}
	}

	protected virtual Task<TDto> InitializeNewItem(TDto dto) {
		return SourceRepository.Initialize(dto);
	}

	protected virtual bool IsNewItem(TViewModel item) {
		// Default implementation assumes item has an Id property that is 0 for new items
		var idProperty = item.GetType().GetProperty("Id");
		if (idProperty != null) {
			var id = idProperty.GetValue(item);
			return id != null && (int)id == 0;
		}
		return false;
	}

	[RelayCommand]
	public virtual async Task SaveItem(TViewModel item) {
		IsLoading = true;
		try {
			_ = item.IsValidationValid();
			var dto = GetDtoFromViewModel(item);

			if (dto != null) {
				if (IsNewItem(item)) {
					_ = await UPAdditionalDataRepo.DeleteFromCache(SourceRepository, GetDtoFromViewModel(item));
				}
				await SaveItemToRepository(item);
			}
		} finally {
			IsLoading = false;
		}
	}

	protected virtual Task SaveItemToRepository(TViewModel item) {
		return UPAdditionalDataRepo.Save(SourceRepository, GetDtoFromViewModel(item)).RunInBackground();
	}

	protected abstract TDto GetDtoFromViewModel(TViewModel item);

	[RelayCommand]
	public virtual async Task DeleteItem(TViewModel item) {
		IsLoading = true;
		try {
			var dto = GetDtoFromViewModel(item);

			_ = IsNewItem(item)
					? await UPAdditionalDataRepo.DeleteFromCache(SourceRepository, dto)
					: await UPAdditionalDataRepo.Delete(SourceRepository, dto).RunInBackgroundWithResult();

			OnPropertyChanged(hasItemsPropertyName);
			OnPropertyChanged(nameof(HasItems));
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