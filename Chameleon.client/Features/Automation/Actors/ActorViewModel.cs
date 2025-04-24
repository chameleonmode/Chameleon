using Chameleon.AIR.Actors.Models;
using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.client.Features.Automation.Actors;

public partial class ActorViewModel(IActor actor) : ViewModelObjectBase {
  [ObservableProperty]
  IActor actor = actor;
}