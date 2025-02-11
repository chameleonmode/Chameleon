using Chameleon.app.Avalonia.Extensions;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveValidation;
using ReactiveValidation.Extensions;

namespace Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.Identity.ViewModels;
public partial class UPBusinessViewModel : ObservableObjectBase {

	public UPBusinessViewModel(UPBusinessDto business) {
		Id = business.id;
		ProfileId = business.ProfileId;
		Title = business.title;
		Notes = business.Notes;
		Tags = business.Tags;
		WebSite = business.WebSite;
		CompanyName = business.CompanyName;
		Department = business.Department;
		PhoneNumber = business.PhoneNumber;
	}

	[ObservableProperty]
	public int id;

	[ObservableProperty]
	private int? profileId;

	[ObservableProperty]
	public string? title;

	[ObservableProperty]
	private string? companyName;

	[ObservableProperty]
	private string? department;

	[ObservableProperty]
	private string? phoneNumber;

	[ObservableProperty]
	private string? webSite;

	[ObservableProperty]
	private string? notes;

	protected override IObjectValidator GetValidator() {
		var builder = new ValidationBuilder<UPBusinessViewModel>();

		_ = builder.RuleFor(vm => vm.Title).NotEmpty();

		_ = builder.RuleFor(vm => vm.CompanyName).NotEmpty().WithMessage("Company name is requried");

		_ = builder.RuleFor(vm => vm.WebSite)
				.Must(context => context.PropertyValue.IsValidWebUrl())
				.WithMessage("Website is not valid");

		_ = builder.RuleFor(vm => vm.PhoneNumber)
				.Must(context => context.PropertyValue.IsValidPhoneNumber())
				.WithMessage("Phone number is not valid");

		return builder.Build(this);
	}

	public UPBusinessDto ToDto() {
		return new UPBusinessDto() {
			id = Id,
			ProfileId = ProfileId,
			title = Title,
			Notes = Notes,
			Tags = Tags,
			WebSite = WebSite,
			CompanyName = CompanyName,
			Department = Department,
			PhoneNumber = PhoneNumber
		};
	}
}
