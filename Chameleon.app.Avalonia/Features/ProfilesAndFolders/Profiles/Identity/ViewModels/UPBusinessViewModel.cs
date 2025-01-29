using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.Identity.ViewModels;
public partial class UPBusinessViewModel : ObservableObjectBase {

	[ObservableProperty]
	public int id;

	[ObservableProperty]
	private int? profileId;

	[ObservableProperty]
	public string? title;

	[ObservableProperty]
	private string? companyName;

	[ObservableProperty]
	private string? department;

	[ObservableProperty]
	private string? phoneNumber;

	[ObservableProperty]
	private string? webSite;

	[ObservableProperty]
	private string? notes;
}
