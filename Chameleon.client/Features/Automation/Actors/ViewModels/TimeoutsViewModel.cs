using Chameleon.AIR.Actors.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.client.Features.Automation.Actors.ViewModels;
public partial class TimeoutsViewModel : ObservableObject {
	[ObservableProperty] int _default;
	[ObservableProperty] int _wait;
	[ObservableProperty] int _navigate;
	[ObservableProperty] RandoViewModel _naps = new();

	public TimeoutsViewModel() { }
	public TimeoutsViewModel(Timeouts source) {
		Default = source.Default;
		Wait = source.Wait;
		Navigate = source.Navigate;
		Naps = new RandoViewModel(source.Naps);
	}
	public Timeouts ToRecord() => new(Default, Wait, Navigate, Naps.ToRecord());
}
