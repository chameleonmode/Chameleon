using System.Collections.ObjectModel;

using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.CommunityToolkit.MvvM;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.app.Avalonia.Models.Observable;
public partial class ObsAddressDto : ViewModelObjectDto<UPAddressDto> {
	public ObservableCollection<CountryzDto> Countries { get; } = new ObservableCollection<CountryzDto>(CountryzRepo.Instance.Countryz);
	[ObservableProperty]
	public CountryzDto? selectedCountry;
	public ObsAddressDto(UPAddressDto adrez)
	{
		Dto = adrez;
		if (adrez.CountryId != null)
			selectedCountry = Countries.FirstOrDefault(x => x.id == adrez.CountryId);
	}
}
