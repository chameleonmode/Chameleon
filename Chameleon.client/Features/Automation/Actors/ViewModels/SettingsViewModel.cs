using Chameleon.AIR.Actors.Models;
using Chameleon.lib.Util;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.client.Features.Automation.Actors.ViewModels;

public partial class SettingsViewModel(Settings source) : ObservableObject {
	[ObservableProperty] Start start = source.Start;
	[ObservableProperty] Timeouts timeouts = source.Timeouts;

	public Settings ToRecord() {
		Start.Urls = Start.Url?.Split('\n').Where(x => x.IsNot()).Select(x => x.Trim());
		return new(Start, Timeouts);
	}
}
