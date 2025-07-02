using System.Collections.ObjectModel;
using Chameleon.lib.Api.Repos;
using Chameleon.client.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using Chameleon.lib.Api.Dto;

namespace Chameleon.client.Features.Projects.Profiles.Dialogs;

public partial class UserProfileSidePanelViewModel : ViewModelObjectBase {
	private readonly ReadOnlyObservableCollection<UPAddressDto> addresses;
	private readonly ReadOnlyObservableCollection<UPLoginDto> logins;
	private readonly ReadOnlyObservableCollection<UPPersonDto> persons;

	public ObservableCollection<CountryzDto> Countries { get; } = new ObservableCollection<CountryzDto>(CountryzRepo.Instance.Countryz);
	public ReadOnlyObservableCollection<UPAddressDto> ProfileAddresses => addresses;
	public ReadOnlyObservableCollection<UPLoginDto> ProfileLogins => logins;
	public ReadOnlyObservableCollection<UPPersonDto> ProfilePersons => persons;

	[ObservableProperty] UPLoginDto? selectedLogin;
	[ObservableProperty] UPPersonDto? selectedPerson;
	[ObservableProperty] UPAddressDto? selectedAddress;
	[ObservableProperty] ObsProfile? userProfile;

	public string? CountryName => Countries?.FirstOrDefault(x => SelectedAddress?.CountryId == x.id)?.Name;
	public bool HasPersons => ProfilePersons.Count > 0;
	public bool HasAddresses => ProfileAddresses?.Count > 0;
	public bool HasLogins => ProfileLogins?.Count > 0;

	public UserProfileSidePanelViewModel(UserProfileDto up) {
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

	public override Task Init(object? param) {
		return UPAdditionalDataRepo.Instance.Load();
	}

	partial void OnSelectedAddressChanged(UPAddressDto? value) => OnPropertyChanged(nameof(CountryName));
}

