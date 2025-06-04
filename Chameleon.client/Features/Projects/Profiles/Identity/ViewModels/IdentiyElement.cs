
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

public abstract partial class IdentiyElementVM<TDto, TViewModel> : ViewModelObjectBase
		where TDto : UP, new()
		where TViewModel : MappableViewModelBase<TDto> {
	readonly BehaviorSubject<Func<UP, bool>> filter;
	[ObservableProperty] UserProfileIdentityVM userProfile;
	[ObservableProperty] bool isLoading = true;
	[ObservableProperty] bool isNotLoading;

	protected abstract UPRepo<TDto> SourceRepository { get; }
	public ReadOnlyObservableCollection<TViewModel> Items { get; }

	public bool HasNoItems => Items.Count == 0;

	protected virtual TViewModel CreateViewModel(TDto dto) {
		var viewModel = (TViewModel)Activator.CreateInstance(typeof(TViewModel), dto)!;
		return viewModel;
	}

	protected IdentiyElementVM(UserProfileIdentityVM userProfile) {
		this.userProfile = userProfile;
		filter = new BehaviorSubject<Func<UP, bool>>(p => p.ProfileId == UserProfile.Id);

		_ = SourceRepository.Connect().Filter(filter)
		.Transform(CreateViewModel)
		.Bind(out var items)
		.Subscribe((i) => {
			OnPropertyChanged(nameof(HasNoItems));
		});
		Items = items;

		_ = this.WhenValueChanged(x => x.IsLoading)
		.DistinctUntilChanged()
		.Subscribe(isLoad => IsNotLoading = !isLoad);

		AsyncCommandMap["AddItem"] = AddItem;
	}

	public virtual void UpdateFilter() {
		filter.OnNext(filter.Value);
		IsLoading = false;
	}

	public virtual async Task AddItem() {
		if (IsBusy || Items.Any(IsNewItem)) return;
		_ = await InitializeNewItem(new() { ProfileId = UserProfile?.Id });
	}

	protected virtual Task<TDto> InitializeNewItem(TDto dto) {
		return SourceRepository.Initialize(dto);
	}

	protected virtual bool IsNewItem(TViewModel item) {
		// Default implementation assumes item has an Id property that is 0 for new items
		return item.Dto.id == 0;
	}

	public virtual async Task SaveItem(TViewModel item) {
		_ = item.IsValidationValid();
		await SaveItemToRepository(item);
	}

	protected virtual async Task SaveItemToRepository(TViewModel item) {
		if (IsNewItem(item)) _ = await UPAdditionalDataRepo.DeleteFromCache(SourceRepository, item.ToDto()); // TODO: Check if this is needed
		await UPAdditionalDataRepo.Save(SourceRepository, item.ToDto()).RunInBackground();
	}

	[RelayCommand]
	public virtual async Task DeleteItem(TViewModel item) {
		_ = IsNewItem(item)
				? await UPAdditionalDataRepo.DeleteFromCache(SourceRepository, item.ToDto())
				: await UPAdditionalDataRepo.Delete(SourceRepository, item.ToDto()).RunInBackgroundWithResult();
	}

	public virtual void ValidateAll() {
		foreach (var item in Items) {
			_ = item.IsValidationValid();
		}
	}

	public virtual async Task SaveAll() {
		var saveTasks = Items.Select(SaveItem).ToArray();
		await Task.WhenAll(saveTasks);
		await SourceRepository.Load(); // TODO: Check if this is needed
	}
}