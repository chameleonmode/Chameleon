using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.Identity.ViewModels;
public partial class UPAddressViewModel: ObservableObjectBase {

	[ObservableProperty]
	public int id;

	[ObservableProperty]
	private int? profileId;

	[ObservableProperty]
	public string? title;

	[ObservableProperty]
	private int? countryId;

	[ObservableProperty]
	private string? addressLine1;

	[ObservableProperty]
	private string? addressLine2;

	[ObservableProperty]
	private string? city;

	[ObservableProperty]
	private string? state;

	[ObservableProperty]
	private string? zip;

	[ObservableProperty]
	private string? notes;
}
