using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveValidation;
using ReactiveValidation.Extensions;

namespace Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.Identity.ViewModels;
public partial class UPLoginViewModel : ObservableObjectBase {

	public UPLoginViewModel(UPLoginDto login) {
		Id = login.id;
		ProfileId = login.ProfileId;
		Title = login.title;
		Notes = login.Notes;
		Tags = login.Tags;
		WebSite = login.WebSite;
		Email = login.Email;
		UserName = login.UserName;
		Password = login.Password;
	}
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

		_ = builder.RuleFor(vm => vm.UserName).NotEmpty().MaxLength(16).WithMessage("Username is required");

		_ = builder.RuleFor(vm => vm.Email).NotEmpty().WithMessage("Email is requried");

		return builder.Build(this);
	}

	public UPLoginDto ToDto() {
		return new UPLoginDto() {
			id = Id,
			ProfileId = ProfileId,
			title = Title,
			Email = Email,
			Notes = Notes,
			Tags = Tags,
			WebSite = WebSite,
			UserName = UserName,
			Password = Password
		};
	}
}
