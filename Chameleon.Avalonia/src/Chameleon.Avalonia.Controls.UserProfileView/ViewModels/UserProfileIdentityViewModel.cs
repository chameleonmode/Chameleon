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

namespace Chameleon.Avalonia.Controls.UserProfileView.ViewModels;

public partial class UserProfileIdentityViewModel : SubPageViewModelBase,
    IUserProfileIdentityViewModel
{

    private readonly IMapper _mapper;
    private readonly IUserProfileService _userProfileService;
    private readonly IUserProfileAdditionalDataService _userProfileAdditionalDataService;
    private readonly IUserAssistantService _userAssistantService;
    private readonly IApplicationUser _applicationUser;
    //TODO: private readonly IFeatureTourNavigator _featureTourNavigator;
    private readonly IAuthSession _authSession;
    private readonly IToastNotificationService _toastNotificationService;

    public UserProfileIdentityViewModel(
        IMapper mapper,
        IUserProfileService userProfileService,
        IUserProfileAdditionalDataService userProfileAdditionalDataService,
        IUserAssistantService userAssistantService,
        IApplicationUser applicationUser,
        IAuthSession authSession,
        IToastNotificationService toastNotificationService
        )
    {
        _mapper = mapper;
        _userProfileService = userProfileService;
        _userProfileAdditionalDataService = userProfileAdditionalDataService;
        _userAssistantService = userAssistantService;
        _applicationUser = applicationUser;
        _authSession = authSession;
        _toastNotificationService = toastNotificationService;

        InitializeViewModels();

        Addresses.Binded += Addresses_Binded;

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

        //EventAggregator
        //    .GetEvent<LoginSuccessEvent>()
        //    .SubscribeOnce(OnAuthenticated);

        EventAggregator
            .GetEvent<UserProfileTabChangedEvent>()
            .Subscribe(Discard);

        //_featureTourNavigator = FeatureTour.GetNavigator();

        //_featureTourNavigator.ForStep(ElementID.SaveChangesBtn).AttachDoable(
        //           currentStep => OnSaveProfile());

    }
    public override async Task InitAsync(object? param)
    {
        await base.InitAsync(param);

        if (!Loaded)
            OnAuthenticated();

       // UserProfile = ContainerServiceHelper.Resolve<IDashboardViewModel>()?.SelectedProfile;
        // OnPropertyChanged(nameof(UserProfileModel));
    }
    public override async Task OnNavigatedToAsync(object? param)
    {
        await base.OnNavigatedToAsync(param);

        if (param is null)
            UserProfile = ContainerServiceHelper.Resolve<IDashboardViewModel>()?.SelectedProfile;
        else if (param is IUserProfile up)
            UserProfile = up;

        Title = UserProfileModel.Title;

    }

    public bool HasNoItems => Persons?.Items?.Count > 0;
    public bool HasNoBusinessItems => Businesses?.Items?.Count > 0;
    public bool HasNoAddressesItems => Addresses?.Items?.Count > 0;
    public bool HasNoLoginsItems => Logins?.Items?.Count > 0;

    private void InitializeViewModels()
    {
        Countries = new AsyncCollectionViewModel<CountryBindable>(() =>
            _userProfileAdditionalDataService.GetCountries()
        );

        Persons = new AsyncCollectionViewModel<UserProfilePersonBindable>(()
             => _userProfileAdditionalDataService.GetPersons(UserProfileModel.Id)
             );

        Addresses = new AsyncCollectionViewModel<UserProfileAddressBindable>(()
            => _userProfileAdditionalDataService.GetAddresses(UserProfileModel.Id)
            );

        Logins = new AsyncCollectionViewModel<UserProfileLoginBindable>(()
            => _userProfileAdditionalDataService.GetLogins(UserProfileModel.Id)
            );

        Businesses = new AsyncCollectionViewModel<UserProfileBusinessBindable>(()
            => _userProfileAdditionalDataService.GetBusinesses(UserProfileModel.Id)
            );
    }

    private void OpenUserProfileIdentityTab(UserProfileIdentityTab userProfileIdentityTab)
    {
        SelectedTadIndex = (int)userProfileIdentityTab;
        OnPropertyChanged(nameof(SelectedTadIndex));
    }

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

    #region UserProfile

    private IUserProfile _userProfile;
    public IUserProfile UserProfile
    {
        set
        {
            SetProperty(ref _userProfile, value);
            UserProfileModel = _mapper.Map<UserProfileBindable>(value);
            BindUi();
            RestrictConfigurations(_authSession.Permissions);
        }
    }

    public int UserProfileId => _userProfile?.Id ?? 0;

    private void BindUi()
    {
        if (UserProfileModel == null)
        {
            Persons.Clear();
            Businesses.Clear();
            Addresses.Clear();
            Logins.Clear();
            return;
        }

        Countries.Load();

        Logins.Reload();
        Persons.Reload();
        Addresses.Reload();
        Businesses.Reload();

        SetVisible(true);
    }

    private void SetVisible(bool isVisible)
    {
        Logins.IsVisible = isVisible;
        Persons.IsVisible = isVisible;
        Addresses.IsVisible = isVisible;
        Businesses.IsVisible = isVisible;

        Logins.Items.CollectionChanged += CollectionChanged;
        Persons.Items.CollectionChanged += CollectionChanged;
        Addresses.Items.CollectionChanged += CollectionChanged;
        Businesses.Items.CollectionChanged += CollectionChanged;
    }

    private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(HasNoItems));
        OnPropertyChanged(nameof(HasNoLoginsItems));
        OnPropertyChanged(nameof(HasNoAddressesItems));
        OnPropertyChanged(nameof(HasNoBusinessItems));
    }

    private UserProfileBindable _userProfileModel;
    public UserProfileBindable UserProfileModel
    {
        get => _userProfileModel;
        set
        {
            if (SetProperty(ref _userProfileModel, value))
            {
                _userProfileModel.ChangedProperty += UserProfileModel_ChangedProperty;
                SyncBtnVisibilityChange();
            }
        }
    }
    private void UserProfileModel_ChangedProperty(object sender, bool value)
    {
        _isChangedProperty = value;
    }

    private bool _isChangedProperty;

    private bool _isSaving;
    public bool IsSaving
    {
        get => _isSaving;
        private set => SetProperty(ref _isSaving, value);
    }

    [RelayCommand]
    private void SaveChanges()
    {
        if (string.IsNullOrEmpty(UserProfileModel.Title) || 
            string.IsNullOrWhiteSpace(UserProfileModel.Title))
            return;

        IsSaving = true;
        //UserProfileModel.YoutubeSettings.IsChanged = false;
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
        foreach (var item in Logins.Items)
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
        foreach (var item in Persons.Items)
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
        foreach (var item in Addresses.Items)
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
        foreach (var item in Businesses.Items)
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

    #region Commands

    private bool UserProfilesCmdCanExecute()
    {
        return UserProfileModel != null;
    }

    private bool UserProfilesCmdCanExecute<T>(T parameter)
    {
        return UserProfilesCmdCanExecute();
    }
    #endregion

    public AsyncCollectionViewModel<CountryBindable> Countries { get; private set; }

    #region Persons
    public AsyncCollectionViewModel<UserProfilePersonBindable> Persons { get; private set; }

    [RelayCommand]
    private void AddPerson()
    {
        var person = new UserProfilePersonBindable(UserProfileId)
        {
            IsOpenSearchParameters = true
        };

        foreach (var item in Persons.Items)
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
    private void OnDeletePerson(UserProfilePersonBindable person)
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
    public AsyncCollectionViewModel<UserProfileBusinessBindable> Businesses { get; private set; }

    [RelayCommand]
    private void OnAddBusiness()
    {
        var business = new UserProfileBusinessBindable(UserProfileId)
        {
            IsOpenSearchParameters = true
        };

        foreach (var item in Businesses.Items)
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
    private void OnDeleteBusiness(UserProfileBusinessBindable business)
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
    public AsyncCollectionViewModel<UserProfileAddressBindable> Addresses { get; private set; }

    private void Addresses_Binded(object sender, EventArgs e)
    {
        Countries.IsVisible = true;
    }

    [RelayCommand]
    private void OnAddAddress()
    {
        var address = new UserProfileAddressBindable(UserProfileId)
        {
            IsOpenSearchParameters = true
        };

        foreach (var item in Addresses.Items)
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
    public AsyncCollectionViewModel<UserProfileLoginBindable> Logins { get; private set; }

    [RelayCommand]
    private void OnAddLogin()
    {
        var login = new UserProfileLoginBindable(UserProfileId)
        {
            IsOpenSearchParameters = true
        };

        foreach (var item in Logins.Items)
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

    public void OnAuthenticated()
    {
        SyncBtnVisibilityChange();

    }

    #endregion
}
