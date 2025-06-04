using Chameleon.lib.Common.Models.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Chameleon.client.Features.Projects.Profiles.Identity.ViewModels;
public partial class ObsAddressViewModel : UPAddressViewModel {
	public ObservableCollection<CountryzDto> Countries { get; } = new ObservableCollection<CountryzDto>(CountryzRepo.Instance.Countryz);
	[ObservableProperty]
	public CountryzDto? selectedCountry;

	[ObservableProperty]
	private UPAddressViewModel dto;
	public ObsAddressViewModel(UPAddressDto adrez):base(adrez) {
		dto = new UPAddressViewModel(adrez);
		if (adrez.CountryId != null)
			selectedCountry = Countries.FirstOrDefault(x => x.id == adrez.CountryId);
	}
}
