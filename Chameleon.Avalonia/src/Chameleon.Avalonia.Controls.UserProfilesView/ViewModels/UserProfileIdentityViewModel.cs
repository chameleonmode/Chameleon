using System.Collections.Specialized;

using AutoMapper;

using Avalonia.Collections;

using Chameleon.Authorization;
using Chameleon.Avalonia.Common.Extensions;
using Chameleon.Avalonia.Controls.UserProfileView.Models.Country;
using Chameleon.Avalonia.Controls.UserProfileView.Models.Profile;
using Chameleon.Avalonia.Controls.UserProfileView.Services;
using Chameleon.Controls.UserProfileView.Models.Additional;
using Chameleon.CT.Common.Base;
using Chameleon.Domain.Entities;
using Chameleon.Infrastructure.Users;
using Chameleon.Interfaces.App.Assistants.Events;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.App.UserProfiles.Events.Common;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Common;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.lib.Common.Extensions;
using Chameleon.lib.Common.ServiceManagers;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Avalonia.Controls.UserProfileView.ViewModels;

public partial class UserProfileIdentityViewModel : SubPageViewModelBase,
    IUserProfileIdentityViewModel
{
    private readonly IMapper _mapper;
    private readonly IUserProfileService _userProfileService;
    private readonly IUserProfileAdditionalDataService _userProfileAdditionalDataService;
    private readonly IUserAssistantService _userAssistantService;
    private readonly IApplicationUser _applicationUser;
    private readonly IAuthSession _authSession;

    [ObservableProperty]
    private UserProfilesView.ViewModels.UserProfileViewModel profileVM;
    [ObservableProperty]
    private bool isSaving;

    public AvaloniaList<CountryBindable> Countries { get; } = new();
    public AvaloniaList<UserProfilePersonBindable> Persons { get; } = new();
    public AvaloniaList<UserProfileBusinessBindable> Businesses { get; } = new();
    public AvaloniaList<UserProfileLoginBindable> Logins { get; } = new();
    public AvaloniaList<UserProfileAddressBindable> Addresses { get; } = new();

    public bool HasNoItems => Persons?.Count > 0;
    public bool HasNoBusinessItems => Businesses?.Count > 0;
    public bool HasNoAddressesItems => Addresses?.Count > 0;
    public bool HasNoLoginsItems => Logins?.Count > 0;
    public int UserProfileId => _userProfile?.Id ?? 0;

    private int _selectedTadIndex;
    public int SelectedTadIndex
    {
        get => _selectedTadIndex;
        set
        {
            if (SetProperty(ref _selectedTadIndex, value))
            {
                Discard();
            }
        }
    }

    public UserProfileIdentityViewModel(
        IMapper mapper,
        IUserProfileService userProfileService,
        IUserProfileAdditionalDataService userProfileAdditionalDataService,
        IUserAssistantService userAssistantService,
        IApplicationUser applicationUser,
        IAuthSession authSession)
    {
        _mapper = mapper;
        _userProfileService = userProfileService;
        _userProfileAdditionalDataService = userProfileAdditionalDataService;
        _userAssistantService = userAssistantService;
        _applicationUser = applicationUser;
        _authSession = authSession;

        SubscribeToEvents();
        InitializeCommands();
    }

    private void SubscribeToEvents()
    {
        EventAggregator.GetEvent<SavedUserProfileEvent>().Subscribe(args => OnUserProfileSaved(args.UserProfile));
        EventAggregator.GetEvent<OpenUserProfileTabEvent>().Subscribe(args => OpenUserProfileIdentityTab(args.UserProfileIdentityTab));
        EventAggregator.GetEvent<RestrictContentEvent>().Subscribe(args => RestrictConfigurations(args.Permissions));
        EventAggregator.GetEvent<SavedUserAssistantEvent>().Subscribe(args => SyncBtnVisibilityChange());
        EventAggregator.GetEvent<DeletedUserAssistantEvent>().Subscribe(args => SyncBtnVisibilityChange());
        EventAggregator.GetEvent<UserProfileTabChangedEvent>().Subscribe(Discard);
        EventAggregator.GetEvent<DeleteUserProfileEvent>().Subscribe(a => NavigationService?.PopAsync());
    }

    private void InitializeCommands()
    {
        CommandMap["AddPerson"] = AddPerson;
        CommandMap["AddBusiness"] = OnAddBusiness;
        CommandMap["AddAddress"] = OnAddAddress;
        CommandMap["AddLogin"] = OnAddLogin;
    }

    public override async Task InitAsync(object? param)
    {
        await base.InitAsync(param);

        if (!Loaded)
        {
            OnAuthenticated();
        }
    }

    public override async Task OnNavigatedToAsync(object? param)
    {
        await base.OnNavigatedToAsync(param);

        if (param is UserProfile up)
        {
            UserProfile = up;
        }

        Title = UserProfileModel.Title;
    }

    private void OpenUserProfileIdentityTab(UserProfileIdentityTab userProfileIdentityTab)
    {
        SelectedTadIndex = (int)userProfileIdentityTab;
        OnPropertyChanged(nameof(SelectedTadIndex));
    }

    [RelayCommand]
    private void Discard()
    {
        UserProfileModel = _mapper.Map<UserProfileBindable>(_userProfile);
    }

    private void OnUserProfileSaved(IUserProfile userProfile)
    {
        if (_userProfile == null || userProfile.Id != _userProfile.Id)
        {
            return;
        }

        UserProfileModel = _mapper.Map<UserProfileBindable>(_userProfile);
    }

    private void UpdateCollectionChangedHandlers(NotifyCollectionChangedEventHandler handler, bool subscribe)
    {
        if (subscribe)
        {
            Addresses.CollectionChanged += handler;
            Logins.CollectionChanged += handler;
            Persons.CollectionChanged += handler;
            Businesses.CollectionChanged += handler;
        }
        else
        {
            Addresses.CollectionChanged -= handler;
            Logins.CollectionChanged -= handler;
            Persons.CollectionChanged -= handler;
            Businesses.CollectionChanged -= handler;
        }
    }

    private async Task BindUi()
    {
        if (UserProfileModel == null)
            return;

        ProfileVM = new UserProfilesView.ViewModels.UserProfileViewModel(
                       _userProfileService,
                       UserProfile,
                       _applicationUser,
                       false);

        Task[] tasks = [
            Addresses.AddNewRangeAsync(() => _userProfileAdditionalDataService.GetAddressesAsync(UserProfileModel.Id)),
            Persons.AddNewRangeAsync(() => _userProfileAdditionalDataService.GetPersonsAsync(UserProfileModel.Id)),
            Logins.AddNewRangeAsync(() => _userProfileAdditionalDataService.GetLoginsAsync(UserProfileModel.Id)),
            Businesses.AddNewRangeAsync(() => _userProfileAdditionalDataService.GetBusinessesAsync(UserProfileModel.Id))
        ];

        CollectionChanged(this, null);

        UpdateCollectionChangedHandlers(CollectionChanged, false);
        UpdateCollectionChangedHandlers(CollectionChanged, true);

        try
        {
            if (Countries.Count == 0)
                await Countries.AddNewRangeAsync(() => Task.Run(_userProfileAdditionalDataService.GetCountries));

            await Task.WhenAll(tasks);
            foreach (var a in Addresses)
                a.SelectedCountry = Countries.FirstOrDefault(x => a?.CountryId == x.Id);
        }
        catch (Exception ex)
        {
			Toaster.ShowErr(ex.Message);
        }
    }

    private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(HasNoItems));
        OnPropertyChanged(nameof(HasNoLoginsItems));
        OnPropertyChanged(nameof(HasNoAddressesItems));
        OnPropertyChanged(nameof(HasNoBusinessItems));
    }

    private Task SaveCollections => Task.Run(() =>
    {
        SaveCollection(Logins, i => i.IsPropertyChanged, OnSaveLogin);
        SaveCollection(Persons, i => i.IsPropertyChanged, OnSavePerson);
        SaveCollection(Addresses, i => i.IsPropertyChanged, OnSaveAddress);
        SaveCollection(Businesses, i => i.IsPropertyChanged, OnSaveBusiness);
    });

    static void SaveCollection<T>(IEnumerable<T> collection, Func<T, bool> isPropChanged, Action<T> saveAction)
    {
        foreach (var item in collection)
        {
            if (isPropChanged(item))
            {
                saveAction(item);
            }
        }
    }

    #region UserProfile

    private UserProfile _userProfile;
    public UserProfile UserProfile
    {
        get => _userProfile;
        set
        {
            if (SetProperty(ref _userProfile, value))
            {
                UserProfileModel = _mapper.Map<UserProfileBindable>(value);
                _ = BindUi();
                RestrictConfigurations(_authSession.Permissions);
            }
        }
    }

    private UserProfileBindable _userProfileModel;
    public UserProfileBindable UserProfileModel
    {
        get => _userProfileModel;
        set
        {
            if (SetProperty(ref _userProfileModel, value))
            {
                SyncBtnVisibilityChange();
            }
        }
    }

    [RelayCommand]
    private async Task SaveChanges()
    {
        if (!UserProfileModel.Title.Is())
            return;

        IsSaving = true;


        try
        {
            await SaveCollections;

            //TODO: check valid for saving only valid data (postoped / agreed)
            UserProfile userProfile = _mapper.Map<UserProfile>(UserProfileModel);

            await Task.Run(() => _userProfileService.Save(userProfile));
        }
        catch (Exception ex)
        {
			// Handle the exception (e.g., log it, show a notification, etc.)
			Toaster.ShowErr($"{ex.Message}");
        }
        finally
        {
            // Code to execute after the task completes, regardless of success or failure
            IsSaving = false;
        }
    }


    #endregion

    #region Persons

    [RelayCommand]
    private void AddPerson()
    {
        var person = new UserProfilePersonBindable(UserProfileId)
        {
            IsOpenSearchParameters = true
        };

        foreach (var item in Persons)
        {
            item.IsOpenSearchParameters = false;
        }

        Persons.Add(person);
        OnPropertyChanged(nameof(HasNoItems));
    }

    [RelayCommand]
    private void OnSavePerson(UserProfilePersonBindable person)
    {
        if (person.Id > 0)
        {
            _userProfileAdditionalDataService.SavePerson(person);
        }
        else
        {
            person.Id = _userProfileAdditionalDataService.AddPerson(person);
        }
        person.IsPropertyChanged = false;
    }

    [RelayCommand]
    private void DeletePerson(UserProfilePersonBindable person)
    {
        if (person.Id > 0)
        {
            _userProfileAdditionalDataService.DeletePerson(person);
        }
        Persons.Remove(person);
        OnPropertyChanged(nameof(HasNoItems));
    }
    #endregion

    #region Business                                                              

    [RelayCommand]
    private void OnAddBusiness()
    {
        var business = new UserProfileBusinessBindable(UserProfileId)
        {
            IsOpenSearchParameters = true
        };

        foreach (var item in Businesses)
        {
            item.IsOpenSearchParameters = false;
        }

        Businesses.Add(business);
        OnPropertyChanged(nameof(HasNoBusinessItems));
    }

    [RelayCommand]
    private void OnSaveBusiness(UserProfileBusinessBindable business)
    {
        if (business.Id > 0)
        {
            _userProfileAdditionalDataService.SaveBusiness(business);
        }
        else
        {
            business.Id = _userProfileAdditionalDataService.AddBusiness(business);
        }
        business.IsPropertyChanged = false;
    }

    [RelayCommand]
    private void DeleteBusiness(UserProfileBusinessBindable business)
    {
        if (business.Id > 0)
        {
            _userProfileAdditionalDataService.DeleteBusiness(business);
        }
        Businesses.Remove(business);
        OnPropertyChanged(nameof(HasNoBusinessItems));
    }
    #endregion

    #region Addresses
    [RelayCommand]
    private void OnAddAddress()
    {
        var address = new UserProfileAddressBindable(UserProfileId)
        {
            IsOpenSearchParameters = true
        };

        foreach (var item in Addresses)
        {
            item.IsOpenSearchParameters = false;
        }

        Addresses.Add(address);
        OnPropertyChanged(nameof(HasNoAddressesItems));
    }

    [RelayCommand]
    private void OnSaveAddress(UserProfileAddressBindable address)
    {
        if (address.Id > 0)
        {
            _userProfileAdditionalDataService.SaveAddress(address);
        }
        else
        {
            address.Id = _userProfileAdditionalDataService.AddAddress(address);
        }
        address.IsPropertyChanged = false;
    }

    [RelayCommand]
    private void OnDeleteAddress(UserProfileAddressBindable address)
    {
        if (address.Id > 0)
        {
            _userProfileAdditionalDataService.DeleteAddress(address);
        }
        Addresses.Remove(address);
        OnPropertyChanged(nameof(HasNoAddressesItems));
    }
    #endregion

    #region Logins     

    [RelayCommand]
    private void OnAddLogin()
    {
        var login = new UserProfileLoginBindable(UserProfileId)
        {
            IsOpenSearchParameters = true
        };

        foreach (var item in Logins)
        {
            item.IsOpenSearchParameters = false;
        }

        Logins.Add(login);
        OnPropertyChanged(nameof(HasNoLoginsItems));
    }

    [RelayCommand]
    private void OnSaveLogin(UserProfileLoginBindable login)
    {
        if (login.Id > 0)
        {
            _userProfileAdditionalDataService.SaveLogin(login);
        }
        else
        {
            login.Id = _userProfileAdditionalDataService.AddLogin(login);
        }
        login.IsPropertyChanged = false;
    }

    [RelayCommand]
    private void OnDeleteLogin(UserProfileLoginBindable login)
    {
        if (login.Id > 0)
        {
            _userProfileAdditionalDataService.DeleteLogin(login);
        }
        Logins.Remove(login);
        OnPropertyChanged(nameof(HasNoLoginsItems));
    }
    #endregion

    #region Main Configuration

    private void RestrictConfigurations(string[] permissions)
    {
        bool isShared = _userProfileService.IsSharedProfile(_userProfile);
        IsProxyConfigVisible = !isShared && permissions.Contains(PermissionNames.Pages_Proxy_Config);
        IsCurateConfigVisible = !isShared && permissions.Contains(PermissionNames.Pages_Curate_Config);
        IsYouTubeConfigVisible = !isShared && permissions.Contains(PermissionNames.Pages_YouTube_Config);
    }

    private bool _isProxyConfigVisible;
    public bool IsProxyConfigVisible
    {
        get => _isProxyConfigVisible;
        set => SetProperty(ref _isProxyConfigVisible, value);
    }

    private bool _isCurateConfigVisible;
    public bool IsCurateConfigVisible
    {
        get => _isCurateConfigVisible;
        set => SetProperty(ref _isCurateConfigVisible, value);
    }

    private bool _isYouTubeConfigVisible;
    public bool IsYouTubeConfigVisible
    {
        get => _isYouTubeConfigVisible;
        set => SetProperty(ref _isYouTubeConfigVisible, value);
    }

    #endregion

    #region Synchronization
    [RelayCommand]
    private void SyncChanges()
    {
        EventAggregator
            .GetEvent<OpenUserProfileEvent>()
            .Publish(new UserProfileEventArgs(_userProfile));

		Toaster.ShowSuccess("Synchronization is completed");
    }

    private void SyncBtnVisibilityChange()
    {
        OnPropertyChanged(nameof(IsSyncChangesBtnVisible));
    }

    private bool HasAssistants()
    {
        return _applicationUser.IsAuthenticated && _userAssistantService.Get().Count > 0;
    }
    public bool IsSyncChangesBtnVisible => _applicationUser.IsAssistant ? _userProfileService.IsSharedProfile(_userProfile) : HasAssistants();
    #endregion

    public void OnAuthenticated()
    {
        SyncBtnVisibilityChange();
    }
}
