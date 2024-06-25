using Chameleon.Avalonia.Common.Services;
using Chameleon.Avalonia.Controls.UserProfileView.Models.Country;
using Chameleon.Avalonia.Controls.UserProfileView.Services;
using Chameleon.Controls.UserProfileView.Models.Additional;
using Chameleon.CT.Common.Base;
using Chameleon.CT.Common.Collections;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.UserProfiles;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Avalonia.Controls.UserProfileView.ViewModels;

public partial class UserProfileSidePanelViewModel : SubPageViewModelBase, IUserProfileSidePanelViewModel
{
    private readonly IUserProfileAdditionalDataService _userProfileAdditionalDataService;
    private IReadOnlyList<CountryBindable> _countries;

    public UserProfileSidePanelViewModel(
        IUserProfileAdditionalDataService userProfileAdditionalDataService)
    {
        _userProfileAdditionalDataService = userProfileAdditionalDataService;

        Logins = new AsyncCollectionViewModel<UserProfileLoginBindable>(()
                => _userProfileAdditionalDataService.GetLogins(UserProfile.Id));

        
    }
    public override async Task InitAsync(object? param)
    {
        await base.InitAsync(param);

        if (!Loaded)
        {
            await Logins.Load();
            _countries = await Task.Run(() => _userProfileAdditionalDataService.GetCountries());
            Loader();
        }
        OnPropertyChanged(nameof(string.Empty));
    }

    public string CountryName => _countries.Where(x => SelectedAddress?.CountryId == x.Id).FirstOrDefault()?.Name;

    public bool HasNoItems => ProfilePersons?.Count > 0;
    public bool HasNoAddressesItems => ProfileAddresses?.Count > 0;
    public bool HasNoLoginsItems => ProfileLogins?.Count > 0;

    public AsyncCollectionViewModel<UserProfileLoginBindable> Logins { get;  }

    private List<UserProfileLoginBindable> _profileLogins;
    public List<UserProfileLoginBindable> ProfileLogins
    {
        get => _profileLogins;
        set => SetProperty(ref _profileLogins, value);
    }

    private UserProfileLoginBindable _selectedLogin;
    public UserProfileLoginBindable SelectedLogin
    {
        get => _selectedLogin;
        set => SetProperty(ref _selectedLogin, value);
    }

    private List<UserProfilePersonBindable> _profilePersons;
    public List<UserProfilePersonBindable> ProfilePersons
    {
        get => _profilePersons;
        set => SetProperty(ref _profilePersons, value);
    }

    private UserProfilePersonBindable _selectedPerson;
    public UserProfilePersonBindable SelectedPerson
    {
        get => _selectedPerson;
        set => SetProperty(ref _selectedPerson, value);
    }

    private List<UserProfileAddressBindable> _profileAddresses;
    public List<UserProfileAddressBindable> ProfileAddresses
    {
        get => _profileAddresses;
        set => SetProperty(ref _profileAddresses, value);
    }

    private UserProfileAddressBindable _selectedAddress;
    public UserProfileAddressBindable SelectedAddress
    {
        get => _selectedAddress;
        set
        {
            if (SetProperty(ref _selectedAddress, value))
            {
                OnPropertyChanged(nameof(CountryName));
            }
        }
    }

    private IUserProfile _userProfile;
    public IUserProfile UserProfile
    {
        set
        {
            SetProperty(ref _userProfile, value);

            if (value != null && Loaded)
            {
                Loader();
            }
        }
        get => _userProfile;
    }
    void Loader()
    {
        ProfileLogins = _userProfileAdditionalDataService.GetLogins(UserProfile.Id, false).ToList();
        SelectedLogin = ProfileLogins.FirstOrDefault(); 

        ProfilePersons = _userProfileAdditionalDataService.GetPersons(UserProfile.Id, false).ToList();
        SelectedPerson = ProfilePersons.FirstOrDefault();

        ProfileAddresses = _userProfileAdditionalDataService.GetAddresses(UserProfile.Id, false).ToList();
        SelectedAddress = ProfileAddresses.FirstOrDefault();

        OnPropertyChanged(nameof(HasNoItems));
        OnPropertyChanged(nameof(HasNoAddressesItems));
        OnPropertyChanged(nameof(HasNoLoginsItems));
    }

    [RelayCommand]
    private void Copy(object param)
    {
        ClipboardService.Instance.SetTextAsync(param as string);
    }
}
