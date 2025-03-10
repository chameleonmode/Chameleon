using System.Collections.ObjectModel;

using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.CommunityToolkit.MvvM;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.app.Avalonia.Models.Observable;
public partial class ObsAddressDto : DtoViewModelBase<UPAddressDto> {
	public ObservableCollection<CountryzDto> Countries { get; } = [.. CountryzRepo.Instance.Countryz];
	[ObservableProperty]
	public CountryzDto? selectedCountry;
	public ObsAddressDto(UPAddressDto adrez) : base(adrez) {
		if (adrez.CountryId != null)
			selectedCountry = Countries.FirstOrDefault(x => x.id == adrez.CountryId);
	}
}
