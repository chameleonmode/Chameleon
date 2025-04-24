using System.Collections.ObjectModel;
using Chameleon.AIR.Actors.Models;
using Chameleon.lib.CommunityToolkit.MvvM;
using RedditActor = Chameleon.AIR.Actors.Models.Reddit.Actor;

namespace Chameleon.client.Features.Automation.Actors;

public partial class ActorsViewModel : ViewModelObjectBase {
  public ObservableCollection<IActor> Actors = [new RedditActor()];
  public ActorsViewModel() : base("Mr. AI-Agent") {

  }
}
