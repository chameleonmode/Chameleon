using Chameleon.AIR.Actors.Models;
using Chameleon.lib.Util;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.client.Features.Automation.Actors.ViewModels;

public partial class SettingsViewModel(AIR.Actors.Models.Settings source) : ObservableObject {
	[ObservableProperty] Start start = source.Start;
	[ObservableProperty] Timeouts timeouts = source.Timeouts;
	[ObservableProperty] bool asQue;
	[ObservableProperty] int rando;
	[ObservableProperty] int delay = 120;
	[ObservableProperty] bool eachProfile;
	[ObservableProperty] bool closeOldBrowserProfileAfterRun;
	public AIR.Actors.Models.Settings ToRecord(Rando? rando = null) {
		Start.Rando = rando ?? new(Rando, Rando);
		Timeouts.Artifacto["delay"] = Delay;
		return new(Start, Timeouts);
	}
}
