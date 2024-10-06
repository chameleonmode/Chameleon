namespace Chameleon.Avalonia.Controls.UserProfileView.ViewModels;

public partial class UserProfileSidePanelViewModel(IUserProfileAdditionalDataService userProfileAdditionalDataService)
	: SubPageViewModelBase, IUserProfileSidePanelViewModel {

	private IEnumerable<CountryBindable> _countries;

	[ObservableProperty]
	private List<UserProfileLoginBindable> profileLogins;
	[ObservableProperty]
	private UserProfileLoginBindable? selectedLogin;
	[ObservableProperty]
	private List<UserProfilePersonBindable> profilePersons;
	[ObservableProperty]
	private UserProfilePersonBindable? selectedPerson;
	[ObservableProperty]
	private List<UserProfileAddressBindable> profileAddresses;
	[ObservableProperty]
	private UserProfileAddressBindable? selectedAddress;
	[ObservableProperty]
	private IUserProfile? userProfile;

	public string? CountryName => _countries?.FirstOrDefault(x => SelectedAddress?.CountryId == x.Id)?.Name;
	public bool HasNoItems => ProfilePersons?.Count > 0;
	public bool HasNoAddressesItems => ProfileAddresses?.Count > 0;
	public bool HasNoLoginsItems => ProfileLogins?.Count > 0;
	public override async Task InitAsync(object? param)
	{
		await base.InitAsync(param);

		if (!Loaded) {
			_countries = await Task.Run(userProfileAdditionalDataService.GetCountries);
			await Loader();
		}

		OnPropertyChanged(nameof(HasNoItems));
		OnPropertyChanged(nameof(HasNoAddressesItems));
		OnPropertyChanged(nameof(HasNoLoginsItems));
		OnPropertyChanged(nameof(CountryName));
	}

	private async Task Loader()
	{
		if (UserProfile == null)
			return;

		ProfileLogins = (await userProfileAdditionalDataService.GetLoginsAsync(UserProfile.Id, false)).ToList();
		SelectedLogin = ProfileLogins.FirstOrDefault();

		ProfilePersons = (await userProfileAdditionalDataService.GetPersonsAsync(UserProfile.Id, false)).ToList();
		SelectedPerson = ProfilePersons.FirstOrDefault();

		ProfileAddresses = (await userProfileAdditionalDataService.GetAddressesAsync(UserProfile.Id, false)).ToList();
		SelectedAddress = ProfileAddresses.FirstOrDefault();
	}

	partial void OnSelectedAddressChanged(UserProfileAddressBindable? value) => OnPropertyChanged(nameof(CountryName));
	partial void OnUserProfileChanged(IUserProfile? value)
	{
		if (value != null && Loaded) {
			_ = Loader();
		}
	}
}
