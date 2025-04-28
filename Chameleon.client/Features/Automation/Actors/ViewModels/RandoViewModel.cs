using Chameleon.AIR.Actors.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.client.Features.Automation.Actors.ViewModels;
public partial class RandoViewModel : ObservableObject {

	[ObservableProperty] int _min;
	[ObservableProperty] int _max;
	[ObservableProperty] int? _multiplier;

	public RandoViewModel() { }
	public RandoViewModel(Rando source) {
		Min = source.Min;
		Max = source.Max;
		Multiplier = source.Multiplier;
	}
	public Rando ToRecord() => new(Min, Max, Multiplier);
}
