using Chameleon.client.Features.Projects.Profiles.Identity.ViewModels;
using Chameleon.client.MvvM;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveValidation;
using ReactiveValidation.Extensions;

namespace Chameleon.client.Features.Projects.Profiles.Identity.Logins;
public partial class UPLoginViewModel(UPLoginDto dto) : MappableViewModelBase<UPLoginDto>(dto) {
	[ObservableProperty] string? webSite = dto.WebSite;
	[ObservableProperty] string? email = dto.Email;
	[ObservableProperty] string? userName = dto.UserName;
	[ObservableProperty] string? password = dto.Password;
	[ObservableProperty] string? notes = dto.Notes;

	protected override IObjectValidator GetValidator() {
		var builder = new ValidationBuilder<UPLoginViewModel>();

		_ = builder.RuleFor(vm => vm.UserName)
		.NotEmpty().WithMessage("Username is empty")
		.MaxLength(236).WithMessage("Username length is greater than 236");

		_ = builder.RuleFor(vm => vm.Email)
		.NotEmpty().WithMessage("Email is empty");

		return builder.Build(this);
	}
}

public class LoginsViewModel(UserProfileIdentityVM userProfile) : IdentiyElementVM<UPLoginDto, UPLoginViewModel>(userProfile) {
	protected override UPRepo<UPLoginDto> SourceRepository => UPAdditionalDataRepo.Instance.Loginz;
}