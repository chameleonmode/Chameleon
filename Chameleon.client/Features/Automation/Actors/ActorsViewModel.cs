using System.Collections.ObjectModel;
using Chameleon.client.Features.Automation.Actors.Models;
using Chameleon.lib.CommunityToolkit.MvvM;

namespace Chameleon.client.Features.Automation.Actors;

public partial class ActorsViewModel : ViewModelObjectBase {
  public ObservableCollection<IActor> Actors = [new Models.Reddit.Actor()];
}
