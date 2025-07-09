using System.Collections.ObjectModel;
using Chameleon.client.MvvM;
using Chameleon.lib.Helpers;
using Chameleon.lib.Util;

namespace Chameleon.client.Features.Projects.Profiles.Dialogs;

public class SnapCracklePopViewModel : OOVM {
	SnapCracklePopViewModel() { }
	public ObservableCollection<ObsProfile> RunningList { get; set; } = [];
	public static void Open(ObsProfile obs) {
		DialogBox.ShowTopmost(vm: Instance, v: SnapCracklePopUserControl.Instance,
			initialize: vm => {
				vm.RunningList.AddIfNot(obs);
			},
			onClosed: vm => {
				vm.RunningList.Clear();
			},
			title: "SCP",
			width: 256
		);
	}

	public static SnapCracklePopViewModel Instance { get; } = new SnapCracklePopViewModel();
}

