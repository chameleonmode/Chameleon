using Chameleon.lib.Common.Constants;
using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.Identity.ViewModels;
public partial class UPPersonViewModel: ObservableObjectBase {

	[ObservableProperty]
	public int id;

	[ObservableProperty]
	public int? profileId;

	[ObservableProperty]
	private string? firstName;

	[ObservableProperty]
	private string? lastName;

	[ObservableProperty]
	private string? middleName;

	[ObservableProperty]
	private string? jobTitle;

	[ObservableProperty]
	private string? phoneNumber;

	[ObservableProperty]
	private string? email;

	[ObservableProperty]
	private string? birthPlace;

	[ObservableProperty]
	private string? notes;

	[ObservableProperty]
	private DateTime birthDate = DateTimeOffset.Now.AddYears(-20).DateTime;

	public DateTimeOffset BirthDateOffset => new(BirthDate);

	[ObservableProperty]
	private Enums.GenderType gender = Enums.GenderType.Female;
	public string Gendertext => Gender.ToString();
}
