using Chameleon.lib.AIR.Actors;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.client.Features.Automation.Actors.ViewModels;

public partial class SettingsViewModel() : ObservableObject {
	[ObservableProperty] bool asQue;
	[ObservableProperty] int rando;
	[ObservableProperty] bool eachProfile;
	[ObservableProperty] bool closeOldBrowserProfileAfterRun;
	[ObservableProperty] int delay = 120;
	[ObservableProperty] Start start = new("x", 9, new(1, 1), new(1, 1));
	[ObservableProperty] Timeouts timeouts = new(30, 15, 60, new(256, 512));
	public SettingsViewModel(lib.AIR.Actors.Settings source) : this() => Set(source);
	public void Set(lib.AIR.Actors.Settings source) {
		Start = source.Start;
		Timeouts = source.Timeouts;
	}
	public lib.AIR.Actors.Settings ToRecord(IEnumerable<string>? urls, Rando? rando = null, Rando? variations = null) {
		return new(
			Start with {
				Rando = rando ?? new(Rando, Rando),
				Variations = variations ?? Start.Variations,
				Urls = urls
			},
			Timeouts with {
				Artifacto = new() { ["delay"] = Delay }
			}
		);
	}

	partial void OnAsQueChanged(bool value) => EachProfile = value ? false : EachProfile;
	partial void OnEachProfileChanged(bool value) => AsQue = value ? false : AsQue;
}
