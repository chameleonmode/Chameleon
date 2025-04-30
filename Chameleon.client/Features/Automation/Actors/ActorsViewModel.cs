using System.Collections.ObjectModel;
using Chameleon.lib.CommunityToolkit.MvvM;
using RedditActor = Chameleon.AIR.Actors.Models.Reddit.Actor;

namespace Chameleon.client.Features.Automation.Actors;

public partial class ActorsViewModel : ViewModelObjectBase {
  public ObservableCollection<ActorViewModel> Actors { get; set; } = [
    new ActorViewModel(new RedditActor()),
    // TODO: load from actor state file
    // new ActorViewModel(JS.DeserializeSafely<RedditActor>(Path.Combine(FilePaths.Roboto, "Reddit")) ?? new RedditActor())
  ];
}
