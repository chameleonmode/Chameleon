using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.client.Features.Projects.Profiles.ViewModels;
public partial class UPFolderViewModel : ObservableObjectBase {
	public UPFolderViewModel(UPFolderDto folder) {
		Id = folder.id;
		Title = folder.title;
		Tags = folder.Tags;
		IsFavorite = folder.isFavorite;
		ProfilesCount = folder.profilesCount;
		CreatorUserId = folder.creatorUserId;
	}

	[ObservableProperty] int id;
	[ObservableProperty] string? title;
	[ObservableProperty] bool isFavorite;
	[ObservableProperty] int profilesCount;
	[ObservableProperty] long? creatorUserId;
	[ObservableProperty] string? tags;

	public UPFolderDto ToDto() {
		return new UPFolderDto() {
			id = Id,
			title = Title,
			Tags = Tags,
			isFavorite = IsFavorite,
			profilesCount = ProfilesCount,
			creatorUserId = CreatorUserId
		};
	}
}
