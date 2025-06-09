using System.Collections.ObjectModel;
using Chameleon.client.MvvM;

namespace Chameleon.client.Features.Projects.Profiles.Dialogs;

public class SnapCracklePopViewModel : ViewModelObjectBase {
	public ObservableCollection<ObsProfile> RunningList { get; set; } = [];

	public static SnapCracklePopViewModel Instance { get; } = new SnapCracklePopViewModel();
}

