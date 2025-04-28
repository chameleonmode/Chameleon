using Chameleon.AIR.Actors.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.client.Features.Automation.Actors.ViewModels;
public partial class SettingsViewModel: ObservableObject {

	[ObservableProperty] StartViewModel _start = new();
	[ObservableProperty] TimeoutsViewModel _timeouts = new();
	[ObservableProperty] RandoViewModel _rando = new();
	[ObservableProperty] RandoViewModel _iterations = new();

	public SettingsViewModel() { } 
	public SettingsViewModel(Settings source) {
		Start = new StartViewModel(source.Start);
		Timeouts = new TimeoutsViewModel(source.Timeouts);
		Rando = new RandoViewModel(source.Rando);
		Iterations = new RandoViewModel(source.Iterations);
	}
	public Settings ToRecord() => new(Start.ToRecord(), Timeouts.ToRecord(), Rando.ToRecord(), Iterations.ToRecord());
}
