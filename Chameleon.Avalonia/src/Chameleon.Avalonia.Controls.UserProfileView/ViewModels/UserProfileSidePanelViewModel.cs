using Chameleon.Avalonia.Common.Services;
using Chameleon.Avalonia.Controls.UserProfileView.Models.Country;
using Chameleon.Avalonia.Controls.UserProfileView.Services;
using Chameleon.Controls.UserProfileView.Models.Additional;
using Chameleon.CT.Common.Base;
using Chameleon.CT.Common.Collections;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.UserProfiles;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Avalonia.Controls.UserProfileView.ViewModels;

public partial class UserProfileSidePanelViewModel(IUserProfileAdditionalDataService userProfileAdditionalDataService)
    : SubPageViewModelBase, 
    IUserProfileSidePanelViewModel
{
    private IReadOnlyList<CountryBindable> _countries;

    [ObservableProperty]
    private List<UserProfileLoginBindable> _profileLogins;
    [ObservableProperty]
    private UserProfileLoginBindable _selectedLogin;
    [ObservableProperty]
    private List<UserProfilePersonBindable> _profilePersons;
    [ObservableProperty]
    private UserProfilePersonBindable _selectedPerson;
    [ObservableProperty]
    private List<UserProfileAddressBindable> _profileAddresses;

    public string CountryName => _countries?.FirstOrDefault(x => SelectedAddress?.CountryId == x.Id)?.Name;
    public bool HasNoItems => ProfilePersons?.Count > 0;
    public bool HasNoAddressesItems => ProfileAddresses?.Count > 0;
    public bool HasNoLoginsItems => ProfileLogins?.Count > 0;

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
                _ = Loader();
            }
        }
        get => _userProfile;
    }

    public override async Task InitAsync(object? param)
    {
        await base.InitAsync(param);

        if (!Loaded)
        {
            _countries = await Task.Run(() => userProfileAdditionalDataService.GetCountries());
            await Loader();
        }
        OnPropertyChanged(nameof(string.Empty));
    }

    async Task Loader()
    {
        ProfileLogins = (await userProfileAdditionalDataService.GetLoginsAsync(UserProfile.Id, false)).ToList();
        SelectedLogin = ProfileLogins.FirstOrDefault(); 

        ProfilePersons = (await userProfileAdditionalDataService.GetPersonsAsync(UserProfile.Id, false)).ToList();
        SelectedPerson = ProfilePersons.FirstOrDefault();

        ProfileAddresses = (await userProfileAdditionalDataService.GetAddressesAsync(UserProfile.Id, false)).ToList();
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
