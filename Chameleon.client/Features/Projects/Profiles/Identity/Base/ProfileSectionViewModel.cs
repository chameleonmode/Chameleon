using Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity.ViewModels;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Util;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity.Base;

/// <summary>
/// Base class for Profile section ViewModels (Persons, Businesses, Addresses, Logins)
/// </summary>
/// <typeparam name="TDto">The DTO type (e.g., UPPersonDto)</typeparam>
/// <typeparam name="TViewModel">The ViewModel type (e.g., UPPersonViewModel)</typeparam>
public abstract partial class ProfileSectionViewModel<TDto, TViewModel> : ViewModelObjectBase
    where TDto : UP, new()
    where TViewModel : ViewModelObjectBase
{
    protected readonly BehaviorSubject<Func<UP, bool>> filter;
    protected readonly ReadOnlyObservableCollection<TViewModel> items;
    protected readonly UserProfileViewModel? userProfile;
    protected readonly string collectionPropertyName;
    protected readonly string hasItemsPropertyName;

    /// <summary>
    /// Gets the collection of items in this section
    /// </summary>
    public ReadOnlyObservableCollection<TViewModel> Items => items;

    /// <summary>
    /// Gets whether there are any items in this section
    /// </summary>
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

    /// <summary>
    /// Gets the filter predicate used to filter items by profile ID
    /// </summary>
    public virtual Func<UP, bool> FilterPredicate => p => p.ProfileId == userProfile?.Id;

    /// <summary>
    /// Gets the source repository for this section
    /// </summary>
    protected abstract UPRepo<TDto> SourceRepository { get; }

    /// <summary>
    /// Creates a new ViewModel from a DTO
    /// </summary>
    protected abstract TViewModel CreateViewModel(TDto dto);

    /// <summary>
    /// Creates a new DTO with the profile ID set
    /// </summary>
    protected virtual TDto CreateDto()
    {
        return new TDto { ProfileId = userProfile?.Id };
    }

    protected ProfileSectionViewModel(
        UserProfileViewModel? userProfile,
        string collectionPropertyName,
        string hasItemsPropertyName)
    {
        this.userProfile = userProfile;
        this.collectionPropertyName = collectionPropertyName;
        this.hasItemsPropertyName = hasItemsPropertyName;
        
        filter = new BehaviorSubject<Func<UP, bool>>(FilterPredicate);

        _ = SourceRepository
            .Connect()
            .Filter(filter)
            .Transform(CreateViewModel)
            .Bind(out items)
            .Subscribe((i) => {
                OnPropertyChanged(hasItemsPropertyName);
                OnPropertyChanged(nameof(HasItems));
            });
    }

    /// <summary>
    /// Updates the filter predicate when the user profile changes
    /// </summary>
    public virtual void UpdateFilter()
    {
        filter.OnNext(FilterPredicate);
    }

    /// <summary>
    /// Adds a new item to the collection
    /// </summary>
    [RelayCommand]
    public virtual async Task AddItem()
    {
        if (Items.Any(x => IsNewItem(x)))
        {
            return;
        }

        _ = await InitializeNewItem(CreateDto());
        OnPropertyChanged(hasItemsPropertyName);
        OnPropertyChanged(nameof(HasItems));
    }

    /// <summary>
    /// Initializes a new item in the repository
    /// </summary>
    protected virtual Task<TDto> InitializeNewItem(TDto dto)
    {
        return SourceRepository.Initialize(dto);
    }

    /// <summary>
    /// Determines if an item is new (has not been saved yet)
    /// </summary>
    protected virtual bool IsNewItem(TViewModel item)
    {
        // Default implementation assumes item has an Id property that is 0 for new items
        var idProperty = item.GetType().GetProperty("Id");
        if (idProperty != null)
        {
            var id = idProperty.GetValue(item);
            return id != null && (int)id == 0;
        }
        return false;
    }

    /// <summary>
    /// Saves an item to the repository
    /// </summary>
    [RelayCommand]
    public virtual async Task SaveItem(TViewModel item)
    {
        _ = item.IsValidationValid();
        await SaveItemToRepository(item);
        
        if (IsNewItem(item))
        {
            _ = await UPAdditionalDataRepo.DeleteFromCache(SourceRepository, GetDtoFromViewModel(item));
        }
    }

    /// <summary>
    /// Saves an item to the repository
    /// </summary>
    protected virtual Task SaveItemToRepository(TViewModel item)
    {
        return UPAdditionalDataRepo.Save(SourceRepository, GetDtoFromViewModel(item)).RunInBackground();
    }

    /// <summary>
    /// Gets the DTO from a ViewModel
    /// </summary>
    protected abstract TDto GetDtoFromViewModel(TViewModel item);

    /// <summary>
    /// Deletes an item from the repository
    /// </summary>
    [RelayCommand]
    public virtual async Task DeleteItem(TViewModel item)
    {
        TDto dto = GetDtoFromViewModel(item);
        
        _ = IsNewItem(item)
            ? await UPAdditionalDataRepo.DeleteFromCache(SourceRepository, dto)
            : await UPAdditionalDataRepo.Delete(SourceRepository, dto).RunInBackgroundWithResult();
            
        OnPropertyChanged(hasItemsPropertyName);
        OnPropertyChanged(nameof(HasItems));
    }

    /// <summary>
    /// Validates all items in the collection
    /// </summary>
    public virtual void ValidateAll()
    {
        foreach (var item in Items)
        {
            item.IsValidationValid();
        }
    }

    /// <summary>
    /// Saves all items in the collection
    /// </summary>
    public virtual async Task SaveAll()
    {
        // Take a snapshot of the collection to avoid "Collection was modified" exception
        var itemsToSave = Items.ToList();
        
        foreach (var item in itemsToSave)
        {
            await SaveItem(item);
        }
    }
}