using Chameleon.AIR.Actors.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.client.Features.Automation.Actors.ViewModels;

public partial class SettingsViewModel() : ObservableObject {
	[ObservableProperty] Start? start;
	[ObservableProperty] Timeouts? timeouts;
	[ObservableProperty] bool asQue;
	[ObservableProperty] int rando;
	[ObservableProperty] int delay = 120;
	[ObservableProperty] bool eachProfile;
	[ObservableProperty] bool closeOldBrowserProfileAfterRun;
	public SettingsViewModel(AIR.Actors.Models.Settings source): this() => Set(source);
	public void Set(AIR.Actors.Models.Settings source) {
		Start = source.Start;
		Timeouts = source.Timeouts;
	}
	public AIR.Actors.Models.Settings ToRecord(IEnumerable<string>? urls = null, Rando? rando = null, Rando? variations = null) {
		return new(
			Start! with {
				Rando = rando ?? new(Rando, Rando),
				Variations = variations ?? Start.Variations,
				Urls = urls ?? Start.Urls
			},
			Timeouts! with {
				Artifacto = new() { ["delay"] = Delay }
			}
		);
	}

	partial void OnAsQueChanged(bool value) => EachProfile = value ? false : EachProfile;
	partial void OnEachProfileChanged(bool value) => AsQue = value ? false : AsQue;
}
