using AutoMapper;
using Chameleon.Avalonia.Controls.UserProfileView.Models.Country;
using Chameleon.Avalonia.Controls.UserProfileView.Models.Profile;
using Chameleon.Avalonia.Controls.UserProfileView.Services;
using Chameleon.Controls.UserProfileView.Models.Additional;
using Chameleon.CT.Common.Base;
using Chameleon.CT.Common.Collections;
using Chameleon.Infrastructure.Users;
using Chameleon.Interfaces.App.Assistants.Events;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.App.UserProfiles.Events.Common;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Common;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.UserProfiles;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Specialized;
using Chameleon.Core.Extensions;
using Chameleon.Authorization;
using Chameleon.Interfaces.Dashboard;
using Chameleon.Domain.Entities;
using Chameleon.Common.Helpers;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Chameleon.Interfaces.WebBrowser;
using Avalonia.Collections;
using Chameleon.Interfaces;
using Chameleon.Avalonia.Common.Extensions;

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
    private readonly IToastNotificationService _toastNotificationService;
    private readonly ISystemBrowserManager _systemBrowserManager;

    private bool _isChangedProperty;

    [ObservableProperty]
    private UserProfilesView.ViewModels.UserProfileViewModel _profileVM;
    [ObservableProperty]
    private bool _isSaving;

    public AvaloniaList<CountryBindable> Countries { get; } = [];       
    public AvaloniaList<UserProfilePersonBindable> Persons { get; } = [];
    public AvaloniaList<UserProfileBusinessBindable> Businesses { get; } = [];
    public AvaloniaList<UserProfileLoginBindable> Logins { get; } = [];
    public AvaloniaList<UserProfileAddressBindable> Addresses { get; } = [];

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
        IAuthSession authSession,
        ISystemBrowserManager systemBrowserManager,
        IToastNotificationService toastNotificationService
        )
    {
        _systemBrowserManager = systemBrowserManager;
        _mapper = mapper;
        _userProfileService = userProfileService;
        _userProfileAdditionalDataService = userProfileAdditionalDataService;
        _userAssistantService = userAssistantService;
        _applicationUser = applicationUser;
        _authSession = authSession;
        _toastNotificationService = toastNotificationService;

        EventAggregator
             .GetEvent<SavedUserProfileEvent>()
             .Subscribe(args => OnUserProfileSaved(args.UserProfile));

        EventAggregator
            .GetEvent<OpenUserProfileTabEvent>()
            .Subscribe(args => OpenUserProfileIdentityTab(args.UserProfileIdentityTab));

        EventAggregator
            .GetEvent<RestrictContentEvent>()
            .Subscribe(args => RestrictConfigurations(args.Permissions));

        EventAggregator
            .GetEvent<SavedUserAssistantEvent>()
            .Subscribe(args => SyncBtnVisibilityChange());

        EventAggregator
            .GetEvent<DeletedUserAssistantEvent>()
            .Subscribe(args => SyncBtnVisibilityChange());

        EventAggregator
            .GetEvent<UserProfileTabChangedEvent>()
            .Subscribe(Discard);

        EventAggregator
            .GetEvent<DeleteUserProfileEvent>()
            .Subscribe(a=> NavigationService.PopAsync());

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
            //TODO: UserAgent var p = await Task.Run(() => { return _userProfileService.Get(up.Id, false); });
            //await Task.Delay(256);
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
        if (_userProfile == null)
        {
            return;
        }

        if (userProfile.Id != _userProfile.Id)
        {
            return;
        }
        UserProfileModel = _mapper.Map<UserProfileBindable>(_userProfile);
    }

    private async void BindUi()
    {
        if (UserProfileModel == null)
            return;

        ProfileVM = new UserProfilesView.ViewModels.UserProfileViewModel(
                       _userProfileService,
                       UserProfile,
                       _applicationUser,
                       _systemBrowserManager,
                       false);

        if(Countries.Count == 0)
            Countries.AddRange(await Task.Run(_userProfileAdditionalDataService.GetCountries));

        Addresses.AddNewRangeAsync(_userProfileAdditionalDataService.GetAddressesAsync(UserProfileModel.Id));
        Persons.AddNewRangeAsync(_userProfileAdditionalDataService.GetPersonsAsync(UserProfileModel.Id));
        Logins.AddNewRangeAsync(_userProfileAdditionalDataService.GetLoginsAsync(UserProfileModel.Id));
        Businesses.AddNewRangeAsync(_userProfileAdditionalDataService.GetBusinessesAsync(UserProfileModel.Id));

        foreach (var a in Addresses)
            a.SelectedCountry = Countries.FirstOrDefault(x => a?.CountryId == x.Id);

        CollectionChanged(this, null);

        Addresses.CollectionChanged -= CollectionChanged;
        Logins.CollectionChanged -= CollectionChanged;
        Persons.CollectionChanged -= CollectionChanged;
        Businesses.CollectionChanged -= CollectionChanged;

        Addresses.CollectionChanged += CollectionChanged;
        Logins.CollectionChanged += CollectionChanged;
        Persons.CollectionChanged += CollectionChanged;
        Businesses.CollectionChanged += CollectionChanged;
    }

    private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(HasNoItems));
        OnPropertyChanged(nameof(HasNoLoginsItems));
        OnPropertyChanged(nameof(HasNoAddressesItems));
        OnPropertyChanged(nameof(HasNoBusinessItems));
    }

    #region UserProfile

    private UserProfile _userProfile;
    public UserProfile UserProfile
    {
        get => _userProfile;
        set
        {
            SetProperty(ref _userProfile, value);
            UserProfileModel = _mapper.Map<UserProfileBindable>(value);
            BindUi();
            RestrictConfigurations(_authSession.Permissions);
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
                _userProfileModel.ChangedProperty += (s,v) => _isChangedProperty = v;
                SyncBtnVisibilityChange();
            }
        }
    }

    [RelayCommand]
    private void SaveChanges()
    {
        if (string.IsNullOrEmpty(UserProfileModel.Title) || 
            string.IsNullOrWhiteSpace(UserProfileModel.Title))
            return;

        IsSaving = true;
        UserProfile userProfile = null;

        DispatcherService.InvokeOnUiThreadAsync(() =>
        {
            SaveCollections();

            //if (_isChangedProperty)
            //{
                //TODO: check valid for saving only valid data (postoped / agreed)
                //bool isValid = UserProfileModel.Proxy.IsModelValid();
                userProfile = _mapper.Map<UserProfile>(UserProfileModel);
                _userProfileService.Save(userProfile);
            //}
        }, _ =>
        {
            if (_isChangedProperty)
            {
                // _mainWindow.SetContent(_mainWindow.GetContent(), userProfile.Title);
            }
            _isChangedProperty = false;
            IsSaving = false;
            //_featureTourNavigator.IfCurrentStepEquals(ElementID.SaveChangesBtn).GoNext();
        });
    }

    #region SaveCollections
    private void SaveCollections()
    {
        IsSaving = true;
        DispatcherService.InvokeOnUiThreadAsync(() =>
        {
            OnSaveLogins();
            OnSavePersons();
            OnSaveAddresses();
            OnSaveBusinesses();
        }, _ => IsSaving = false);
    }

    private void OnSaveLogins()
    {
        foreach (var item in Logins)
        {
            if (!item.IsPropertyChanged)
            {
                continue;
            }
            OnSaveLogin(item);
        }
    }

    private void OnSavePersons()
    {
        foreach (var item in Persons)
        {
            if (!item.IsPropertyChanged)
            {
                continue;
            }
            OnSavePerson(item);
        }
    }

    private void OnSaveAddresses()
    {
        foreach (var item in Addresses)
        {
            if (!item.IsPropertyChanged)
            {
                continue;
            }
            OnSaveAddress(item);
        }
    }

    private void OnSaveBusinesses()
    {
        foreach (var item in Businesses)
        {
            if (!item.IsPropertyChanged)
            {
                continue;
            }
            OnSaveBusiness(item);
        }
    }
    #endregion


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

        _toastNotificationService.ShowSuccess("Synchronization is completed");
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
