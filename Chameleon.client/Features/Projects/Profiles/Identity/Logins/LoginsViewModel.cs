using Chameleon.client.Features.Projects.Profiles.Identity.ViewModels;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;

namespace Chameleon.client.Features.Projects.Profiles.Identity.Logins;

public partial class LoginsViewModel : ProfileSectionViewModel<UPLoginDto, UPLoginViewModel> {
	protected override UPRepo<UPLoginDto> SourceRepository => UPAdditionalDataRepo.Instance.Loginz;

	protected override UPLoginViewModel CreateViewModel(UPLoginDto dto) => new(dto);

	protected override UPLoginDto GetDtoFromViewModel(UPLoginViewModel item) => item.ToDto();

	public LoginsViewModel(UserProfileViewModel? userProfile)
			: base(userProfile, nameof(Items), nameof(HasItems)) {
		AsyncCommandMap["AddLogin"] = AddItem;
	}

	public static LoginsViewModel Create(UserProfileViewModel? userProfile) {
		return new LoginsViewModel(userProfile);
	}
}