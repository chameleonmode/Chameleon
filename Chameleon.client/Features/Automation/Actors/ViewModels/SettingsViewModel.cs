using Chameleon.AIR.Actors.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.client.Features.Automation.Actors.ViewModels;
public partial class SettingsViewModel(Settings source) : ObservableObject {
	[ObservableProperty] Start start = source.Start;
	[ObservableProperty] Timeouts timeouts = source.Timeouts;

	public Settings ToRecord() => new(Start, Timeouts);
}
