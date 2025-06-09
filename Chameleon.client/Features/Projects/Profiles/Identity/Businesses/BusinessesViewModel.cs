using Chameleon.client.Features.Projects.Profiles.Identity.ViewModels;
using Chameleon.lib.Util;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveValidation;
using ReactiveValidation.Extensions;
using Chameleon.client.MvvM;

namespace Chameleon.client.Features.Projects.Profiles.Identity.Businesses;

public partial class UPBusinessViewModel(UPBusinessDto dto) : MappableViewModelBase<UPBusinessDto>(dto) {
	[ObservableProperty] string? companyName = dto.CompanyName;
	[ObservableProperty] string? department = dto.Department;
	[ObservableProperty] string? phoneNumber = dto.PhoneNumber;
	[ObservableProperty] string? webSite = dto.WebSite;
	[ObservableProperty] string? notes = dto.Notes;

	protected override IObjectValidator GetValidator() {
		var builder = new ValidationBuilder<UPBusinessViewModel>();

		_ = builder.RuleFor(vm => vm.Title)
		.NotEmpty();

		_ = builder.RuleFor(vm => vm.CompanyName)
		.NotEmpty()
		.WithMessage("Company name is empty");

		_ = builder.RuleFor(vm => vm.WebSite)
		.Must(context => context.PropertyValue.IsValidWebUrl())
		.WithMessage("Website is not valid");

		_ = builder.RuleFor(vm => vm.PhoneNumber)
		.Must(context => context.PropertyValue.IsValidPhoneNumber())
		.WithMessage("Phone number is not valid");

		return builder.Build(this);
	}
}
public class BusinessesViewModel(UserProfileIdentityVM userProfile) : IdentiyElementVM<UPBusinessDto, UPBusinessViewModel>(userProfile) {
	protected override UPRepo<UPBusinessDto> SourceRepository => UPAdditionalDataRepo.Instance.Biz;
}