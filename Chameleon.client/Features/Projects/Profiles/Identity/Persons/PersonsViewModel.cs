using Chameleon.client.Features.Projects.Profiles.Identity.ViewModels;
using Chameleon.lib.Util;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.Models.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveValidation;
using ReactiveValidation.Extensions;
using Chameleon.client.Libs.MvvM;

namespace Chameleon.client.Features.Projects.Profiles.Identity.Persons;
public partial class UPPersonViewModel(UPPersonDto dto) : MappableViewModelBase<UPPersonDto>(dto) {
	[ObservableProperty] int? profileId = dto.ProfileId;
	[ObservableProperty] string? firstName = dto.FirstName;
	[ObservableProperty] string? lastName = dto.LastName;
	[ObservableProperty] string? middleName = dto.MiddleName;
	[ObservableProperty] string? jobTitle = dto.JobTitle;
	[ObservableProperty] string? phoneNumber = dto.PhoneNumber;
	[ObservableProperty] string? email = dto.Email;
	[ObservableProperty] string? birthPlace = dto.BirthPlace;
	[ObservableProperty] string? notes = dto.Notes;
	[ObservableProperty] Enums.GenderType gender = dto.Gender;
	[ObservableProperty] DateTime birthDate = dto.BirthDate;

	public DateTimeOffset BirthDateOffset => new(BirthDate);

	public string Gendertext => Gender.ToString();

	protected override IObjectValidator GetValidator() {
		var builder = new ValidationBuilder<UPPersonViewModel>();

		_ = builder.RuleFor(vm => vm.Title)
		.NotEmpty().MaxLength(50);

		_ = builder.RuleFor(vm => vm.FirstName)
		.NotEmpty().WithMessage("Firstname is empty");

		_ = builder.RuleFor(vm => vm.LastName)
		.NotEmpty().WithMessage("Lastname is empty");

		_ = builder.RuleFor(vm => vm.PhoneNumber)
		.Must(context => context.PropertyValue.IsValidPhoneNumber())
		.WithMessage("Consider a valid phone number");

		return builder.Build(this);
	}
}

public class PersonsViewModel(UserProfileViewModel userProfile) : ProfileSectionViewModel<UPPersonDto, UPPersonViewModel>(userProfile) {
	protected override UPRepo<UPPersonDto> SourceRepository => UPAdditionalDataRepo.Instance.Personz;
	protected override UPPersonViewModel CreateViewModel(UPPersonDto dto) => new(dto);

	public static PersonsViewModel Create(UserProfileViewModel userProfile) {
		return new PersonsViewModel(userProfile);
	}
}