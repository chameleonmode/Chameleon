using Chameleon.client.Features.Projects.Profiles.Identity.ViewModels;
using Chameleon.client.MvvM;
using Chameleon.lib.Api.Dto;
using Chameleon.lib.Api.Repos;
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

		_ = builder.RuleFor(vm => vm.WebSite)
		.Must(uri => {
			if (string.IsNullOrEmpty(uri)) {
				return true; 
			}
			var potentialUri = uri.Contains("://") ? uri : $"http://{uri}";
			return Uri.TryCreate(potentialUri, UriKind.Absolute, out var resultUri) && (resultUri.HostNameType == UriHostNameType.Dns
				? resultUri.Host.Contains('.')
				: resultUri.HostNameType is UriHostNameType.IPv4 or UriHostNameType.IPv6);
		})
		.WithMessage("Please enter a valid website URL (e.g., example.com)");

		return builder.Build(this);
	}
}

public class LoginsViewModel(UserProfileIdentityVM userProfile) : IdentiyElementVM<UPLoginDto, UPLoginViewModel>(userProfile) {
	protected override UPRepo<UPLoginDto> SourceRepository => UPAdditionalDataRepo.Instance.Loginz;
}