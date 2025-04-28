using Chameleon.AIR.Actors.Models;
using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using RedditActor = Chameleon.AIR.Actors.Models.Reddit.Actor;

namespace Chameleon.client.Features.Automation.Actors.V2;
public partial class ActorsViewModel: ViewModelObjectBase {
	public ObservableCollection<ActorViewModel> Actors { get; set; } = [new ActorViewModel(new RedditActor())];
	public ActorsViewModel() : base("Mr. AI-Robot") {

	}

	[RelayCommand]
	public void Open(IActor actor) {
		if (actor is RedditActor redditActor) {

		}
	}
}
