using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveValidation;
using ReactiveValidation.Extensions;

namespace Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity.ViewModels;
public partial class UPAddressViewModel: ViewModelObjectBase {

	public UPAddressViewModel(UPAddressDto address) {
		Id = address.id;
		ProfileId = address.ProfileId;
		Title = address.title;
		Notes = address.Notes;
		Tags = address.Tags;
		CountryId = address.CountryId;
		AddressLine1 = address.AddressLine1;
		AddressLine2 = address.AddressLine2;
		City = address.City;
		State = address.State;
		Zip = address.Zip;
	}

	[ObservableProperty]
	public int id;

	[ObservableProperty]
	private int? profileId;

	[ObservableProperty]
	public string? title;

	[ObservableProperty]
	private int? countryId;

	[ObservableProperty]
	private string? addressLine1;

	[ObservableProperty]
	private string? addressLine2;

	[ObservableProperty]
	private string? city;

	[ObservableProperty]
	private string? state;

	[ObservableProperty]
	private string? zip;

	[ObservableProperty]
	private string? notes;

	protected override IObjectValidator GetValidator() {
		var builder = new ValidationBuilder<UPAddressViewModel>();

		_ = builder.RuleFor(vm => vm.Title).NotEmpty().MaxLength(50);

		_ = builder.RuleFor(vm => vm.AddressLine1).NotEmpty().WithMessage("AddressLine is empty");

		_ = builder.RuleFor(vm => vm.City).NotEmpty().WithMessage("City is empty");

		_ = builder.RuleFor(vm => vm.State).NotEmpty().WithMessage("State is empty");

		_ = builder.RuleFor(vm => vm.CountryId)
					.Must(x => x.PropertyValue is not null and not 0)
					.WithMessage("Country is empty");

		return builder.Build(this);
	}

	public UPAddressDto ToDto() {
		return new UPAddressDto() {
			id = Id,
			ProfileId = ProfileId,
			title = Title,
			Notes = Notes,
			Tags = Tags,
			CountryId = CountryId,
			AddressLine1 = AddressLine1,
			AddressLine2 = AddressLine2,
			City = City,
			State = State,
			Zip = Zip
		};
	}
}
