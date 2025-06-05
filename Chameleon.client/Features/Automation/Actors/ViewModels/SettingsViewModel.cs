using Chameleon.AIR.Actors.Models;
using Chameleon.lib.Util;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.client.Features.Automation.Actors.ViewModels;

public partial class SettingsViewModel(AIR.Actors.Models.Settings source) : ObservableObject {
	[ObservableProperty] Start start = source.Start;
	[ObservableProperty] Timeouts timeouts = source.Timeouts;
	[ObservableProperty] int randomWaitPerProfile = 1;
	[ObservableProperty] bool executeOneScriptAccrosProfiles;
	[ObservableProperty] bool closeOldBrowserProfileAfterRun;
	public AIR.Actors.Models.Settings ToRecord(Start? s = null) {
		var tart = s ?? Start;
		tart.Urls = Start.Url?.Split('\n').Where(x => x.IsNot()).Select(x => x.Trim());
		return new(tart, Timeouts);
	}
}
