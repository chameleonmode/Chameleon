using System.Collections.ObjectModel;

using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;

using DynamicData;

namespace Chameleon.app.Avalonia.ViewModels.Controllers;
public partial class UserProfileSidePanelViewModel
	: ViewModelObjectBase {
	private readonly ReadOnlyObservableCollection<UPAddressDto> addresses;
	private readonly ReadOnlyObservableCollection<CountryzDto> countries;
	private readonly ReadOnlyObservableCollection<UPLoginDto> logins;
	private readonly ReadOnlyObservableCollection<UPPersonDto> persons;

	public ReadOnlyObservableCollection<UPAddressDto> ProfileAddresses => addresses;
	public ReadOnlyObservableCollection<CountryzDto> Countries => countries;
	public ReadOnlyObservableCollection<UPLoginDto> ProfileLogins => logins;
	public ReadOnlyObservableCollection<UPPersonDto> ProfilePersons => persons;

	[ObservableProperty]
	private UPLoginDto? selectedLogin;
	[ObservableProperty]
	private UPPersonDto? selectedPerson;
	[ObservableProperty]
	private UPAddressDto? selectedAddress;
	[ObservableProperty]
	private ObsProfile? userProfile;

	public string? CountryName => Countries?.FirstOrDefault(x => SelectedAddress?.CountryId == x.id)?.Name;
	public bool HasPersons => ProfilePersons.Count > 0;
	public bool HasAddresses => ProfileAddresses?.Count > 0;
	public bool HasLogins => ProfileLogins?.Count > 0;

	public UserProfileSidePanelViewModel(UserProfileDto up)
	{
		_ = UPAdditionalDataRepo.Instance.Countryz
			.Connect(i => i.ProfileId == up.id)
			.Bind(out countries)
			.Subscribe();
		//
		_ = UPAdditionalDataRepo.Instance.Personz
			.Connect(i => i.ProfileId == up.id)
			.Bind(out persons)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasPersons));
			});
		//
		_ = UPAdditionalDataRepo.Instance.Loginz
			.Connect(i => i.ProfileId == up.id)
			.Bind(out logins)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasLogins));
			});
		//
		_ = UPAdditionalDataRepo.Instance.Addrez
			.Connect(i => i.ProfileId == up.id)
			.Bind(out addresses)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasAddresses));
			});
		userProfile = new ObsProfile(up);
	}

	private async Task Loader()
	{
		if (UserProfile == null)
			return;

		//ProfileLogins = (await userProfileAdditionalDataService!.GetLoginsAsync(UserProfile.Id, false)).ToList();
		//SelectedLogin = ProfileLogins.FirstOrDefault();

		//ProfilePersons = (await userProfileAdditionalDataService.GetPersonsAsync(UserProfile.Id, false)).ToList();
		//SelectedPerson = ProfilePersons.FirstOrDefault();

		//ProfileAddresses = (await userProfileAdditionalDataService.GetAddressesAsync(UserProfile.Id, false)).ToList();
		//SelectedAddress = ProfileAddresses.FirstOrDefault();
	}

	partial void OnSelectedAddressChanged(UPAddressDto? value) => OnPropertyChanged(nameof(CountryName));
	partial void OnUserProfileChanged(ObsProfile? value)
	{
		if (value != null && Loaded) {
			_ = Loader();
		}
	}
}
