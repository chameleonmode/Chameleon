using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.MyProfiles.ViewModels;
public partial class UPFolderViewModel : ObservableObjectBase {

	public UPFolderViewModel(UPFolderDto folder) {
		Id = folder.id;
		Title = folder.title;
		Tags = folder.Tags;
		IsFavorite = folder.isFavorite;
		ProfilesCount = folder.profilesCount;
		CreatorUserId = folder.creatorUserId;
	}

	[ObservableProperty]
	public int id;

	[ObservableProperty]
	public string? title;

	[ObservableProperty]
	public bool isFavorite;

	[ObservableProperty]
	public int profilesCount;

	[ObservableProperty]
	public long? creatorUserId;

	[ObservableProperty]
	private string? tags;

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
