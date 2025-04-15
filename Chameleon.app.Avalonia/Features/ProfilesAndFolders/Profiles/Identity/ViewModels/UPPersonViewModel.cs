using Chameleon.app.Avalonia.Extensions;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Util;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveValidation;
using ReactiveValidation.Extensions;
using System;

namespace Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.Identity.ViewModels;
public partial class UPPersonViewModel: ViewModelObjectBase {

	public UPPersonViewModel(UPPersonDto person) {
		Id = person.id;
		ProfileId = person.ProfileId;
		Title = person.title;
		FirstName = person.FirstName;
		LastName = person.LastName;
		MiddleName = person.MiddleName;
		Email = person.Email;
		JobTitle = person.JobTitle;
		PhoneNumber = person.PhoneNumber;
		BirthPlace = person.BirthPlace;
		Notes = person.Notes;
		Tags = person.Tags;
		Gender = person.Gender;
		BirthDate = person.BirthDate;
	}

	[ObservableProperty]
	public int id;

	[ObservableProperty]
	public int? profileId;

	[ObservableProperty]
	public string? title;

	[ObservableProperty]
	private string? firstName;

	[ObservableProperty]
	private string? lastName;

	[ObservableProperty]
	private string? middleName;

	[ObservableProperty]
	private string? jobTitle;

	[ObservableProperty]
	private string? phoneNumber;

	[ObservableProperty]
	private string? email;

	[ObservableProperty]
	private string? birthPlace;

	[ObservableProperty]
	private string? notes;

	[ObservableProperty]
	private DateTime birthDate = DateTimeOffset.Now.AddYears(-20).DateTime;

	public DateTimeOffset BirthDateOffset => new(BirthDate);

	[ObservableProperty]
	private Enums.GenderType gender = Enums.GenderType.Female;
	public string Gendertext => Gender.ToString();

	protected override IObjectValidator GetValidator() {
		var builder = new ValidationBuilder<UPPersonViewModel>();

		_ = builder.RuleFor(vm => vm.Title).NotEmpty().MaxLength(50);

		_ = builder.RuleFor(vm => vm.FirstName).NotEmpty().WithMessage("Firstname is empty");

		_ = builder.RuleFor(vm => vm.LastName).NotEmpty().WithMessage("Lastname is empty");

		_ = builder.RuleFor(vm => vm.PhoneNumber)
				.Must(context => context.PropertyValue.IsValidPhoneNumber())
				.WithMessage("Consider a valid phone number");

		return builder.Build(this);
	}

	public UPPersonDto ToDto() {
		return new UPPersonDto() {
			id = Id,
			ProfileId = ProfileId,
			title = Title,
			FirstName = FirstName,
			LastName = LastName,
			MiddleName = MiddleName,
			Email = Email,
			JobTitle = JobTitle,
			PhoneNumber = PhoneNumber,
			BirthPlace = BirthPlace,
			Notes = Notes,
			Tags = Tags,
			Gender = Gender,
			BirthDate = BirthDate
		};
	}
}
