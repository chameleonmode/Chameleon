using Chameleon.app.Avalonia.Models.Observable;
using System.Collections.ObjectModel;
using Chameleon.lib.CommunityToolkit.MvvM;

namespace Chameleon.app.Avalonia.ViewModels.Controllers;

public class SnapCracklePopViewModel : ViewModelObjectBase {
	public ObservableCollection<ObsProfile> RunningList { get; set; } = [];

	public static SnapCracklePopViewModel Instance { get; } = new SnapCracklePopViewModel();
}
