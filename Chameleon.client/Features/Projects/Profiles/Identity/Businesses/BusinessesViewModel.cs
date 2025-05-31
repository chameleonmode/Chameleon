using Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity.Base;
using Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity.ViewModels;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using DynamicData;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity.Businesses;

public partial class BusinessesViewModel : ProfileSectionViewModel<UPBusinessDto, UPBusinessViewModel> {
	protected override UPRepo<UPBusinessDto> SourceRepository => UPAdditionalDataRepo.Instance.Biz;

	protected override UPBusinessViewModel CreateViewModel(UPBusinessDto dto) => new(dto);

	protected override UPBusinessDto GetDtoFromViewModel(UPBusinessViewModel item) => item.ToDto();

	public BusinessesViewModel(UserProfileViewModel? userProfile)
			: base(userProfile, nameof(Items), nameof(HasItems)) {
		AsyncCommandMap["AddBusiness"] = AddItem;
	}

	public static BusinessesViewModel Create(UserProfileViewModel? userProfile) {
		return new BusinessesViewModel(userProfile);
	}
}