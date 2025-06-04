using Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity.Base;
using Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity.ViewModels;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;

namespace Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity.Addresses;

public partial class AddressesViewModel : ProfileSectionViewModel<UPAddressDto, ObsAddressViewModel> {
	protected override UPRepo<UPAddressDto> SourceRepository => UPAdditionalDataRepo.Instance.Addrez;

	protected override ObsAddressViewModel CreateViewModel(UPAddressDto dto) => new(dto);

	protected override UPAddressDto GetDtoFromViewModel(ObsAddressViewModel item) => item.ToDto();

	public AddressesViewModel(UserProfileViewModel? userProfile)
			: base(userProfile, nameof(Items), nameof(HasItems)) {
		AsyncCommandMap["AddAddress"] = AddItem;
	}

	public static AddressesViewModel Create(UserProfileViewModel? userProfile) {
		return new AddressesViewModel(userProfile);
	}
}