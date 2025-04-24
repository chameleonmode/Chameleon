using System.Collections.ObjectModel;
using Chameleon.AIR.Actors.Models;
using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.Input;
using RedditActor = Chameleon.AIR.Actors.Models.Reddit.Actor;

namespace Chameleon.client.Features.Automation.Actors;

public partial class ActorsViewModel : ViewModelObjectBase {
  public ObservableCollection<IActor> Actors { get; set; } = [new RedditActor()];
  public ActorsViewModel() : base("Mr. AI-Agent") {

  }

  [RelayCommand]
  public void Open(IActor actor) {
    if (actor is RedditActor redditActor) {
      
    }
  }
}
