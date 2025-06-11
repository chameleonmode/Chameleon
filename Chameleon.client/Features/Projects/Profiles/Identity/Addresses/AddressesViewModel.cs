using System.Collections.ObjectModel;
using Chameleon.client.Features.Projects.Profiles.Identity.ViewModels;
using Chameleon.client.MvvM;
using Chameleon.lib.Api.Dto;
using Chameleon.lib.Api.Repos;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveValidation;
using ReactiveValidation.Extensions;

namespace Chameleon.client.Features.Projects.Profiles.Identity.Addresses;

public partial class UPAddressViewModel(UPAddressDto dto) : MappableViewModelBase<UPAddressDto>(dto) {
	[ObservableProperty] int? countryId = dto.CountryId;
	[ObservableProperty] string? addressLine1 = dto.AddressLine1;
	[ObservableProperty] string? addressLine2 = dto.AddressLine2;
	[ObservableProperty] string? city = dto.City;
	[ObservableProperty] string? state = dto.State;
	[ObservableProperty] string? zip = dto.Zip;
	[ObservableProperty] string? notes = dto.Notes;
	[ObservableProperty] CountryzDto? selectedCountry = CountryzRepo.Instance.Countryz
		.FirstOrDefault(x => x.id == dto.CountryId);

	public ObservableCollection<CountryzDto> Countries { get; } = new ObservableCollection<CountryzDto>(CountryzRepo.Instance.Countryz);

	protected override IObjectValidator GetValidator() {
		var builder = new ValidationBuilder<UPAddressViewModel>();
		_ = builder.RuleFor(vm => vm.Title)
		.NotEmpty().MaxLength(50);

		_ = builder.RuleFor(vm => vm.AddressLine1)
		.NotEmpty().WithMessage("AddressLine is empty");

		_ = builder.RuleFor(vm => vm.City)
		.NotEmpty().WithMessage("City is empty");

		_ = builder.RuleFor(vm => vm.State)
		.NotEmpty().WithMessage("State is empty");

		_ = builder.RuleFor(vm => vm.CountryId)
		.Must(x => x.PropertyValue is not null and not 0)
		.WithMessage("Country is empty");

		return builder.Build(this);
	}
}

public class AddressesViewModel(UserProfileIdentityVM userProfile) : IdentiyElementVM<UPAddressDto, UPAddressViewModel>(userProfile) {
	protected override UPRepo<UPAddressDto> SourceRepository => UPAdditionalDataRepo.Instance.Addrez;
}