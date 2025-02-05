using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.MyProfiles.ViewModels;
public partial class UPFolderViewModel : ObservableObjectBase {

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
}
