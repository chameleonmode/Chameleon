using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveValidation;
using ReactiveValidation.Extensions;

namespace Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.Identity.ViewModels;
public partial class UPLoginViewModel: ObservableObjectBase {
	[ObservableProperty]
	public int id;

	[ObservableProperty]
	public string? title;

	[ObservableProperty]
	public int? profileId;

	[ObservableProperty]
	public string? webSite;

	[ObservableProperty]
	public string? email;

	[ObservableProperty]
	public string? userName;

	[ObservableProperty]
	public string? password;

	[ObservableProperty]
	public string? notes;

	protected override IObjectValidator GetValidator() {
		var builder = new ValidationBuilder<UPLoginViewModel>();

		_ = builder.RuleFor(vm => vm.UserName).NotEmpty().MaxLength(16);

		_ = builder.RuleFor(vm => vm.Email).NotEmpty();

		return builder.Build(this);
	}
}
