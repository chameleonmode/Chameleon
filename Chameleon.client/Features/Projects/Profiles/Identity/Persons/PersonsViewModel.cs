using Chameleon.client.Features.Projects.Profiles.Identity.ViewModels;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;

namespace Chameleon.client.Features.Projects.Profiles.Identity.Persons;

public partial class PersonsViewModel : ProfileSectionViewModel<UPPersonDto, UPPersonViewModel> {
	protected override UPRepo<UPPersonDto> SourceRepository => UPAdditionalDataRepo.Instance.Personz;

	protected override UPPersonViewModel CreateViewModel(UPPersonDto dto) => new(dto);

	protected override UPPersonDto GetDtoFromViewModel(UPPersonViewModel item) => item.ToDto();

	public PersonsViewModel(UserProfileViewModel? userProfile)
			: base(userProfile, nameof(Items), nameof(HasItems)) {
		AsyncCommandMap["AddPerson"] = AddItem;
	}

	public static PersonsViewModel Create(UserProfileViewModel? userProfile) {
		return new PersonsViewModel(userProfile);
	}
}