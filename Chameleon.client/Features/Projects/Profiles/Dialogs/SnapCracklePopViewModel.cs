using System.Collections.ObjectModel;
using Chameleon.client.MvvM;
using Chameleon.lib.Api.Dto;
using Chameleon.lib.Helpers;
using Chameleon.lib.Util;

namespace Chameleon.client.Features.Projects.Profiles.Dialogs;

public class SnapCracklePopViewModel : ViewModelObjectBase {
	public ObservableCollection<ObsProfile> RunningList { get; set; } = [];
	public static void Open(UserProfileDto dto) {
		DialogBox.ShowTopmost(vm: Instance, v: SnapCracklePopUserControl.Instance,
			initialize: vm => {
				vm.RunningList.AddIfNot(new ObsProfile(dto) { IsShowGlyph = false, IsShowCheckboxColumn = false }, p => p.Dto?.id == dto.id);
			},
			onClosed: vm => {
				vm.RunningList.Clear();
			},
			title: "SCP",
			width: 172
		);
	}

	public static SnapCracklePopViewModel Instance { get; } = new SnapCracklePopViewModel();
}

