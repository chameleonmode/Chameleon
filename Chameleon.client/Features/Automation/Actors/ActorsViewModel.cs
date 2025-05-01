using System.Collections.ObjectModel;
using System.Diagnostics;
using Chameleon.lib.CommunityToolkit.MvvM;
using RedditActor = Chameleon.AIR.Actors.Models.Reddit.Actor;
using Chameleon.lib.Const;

namespace Chameleon.client.Features.Automation.Actors;

public partial class ActorsViewModel : ViewModelObjectBase {
	public ObservableCollection<ActorViewModel> Actors { get; set; } = [];

	public ActorsViewModel() {
		LoadActorStates();
	}

	private async void LoadActorStates() {
		Actors.Clear();
		
		foreach (var filePath in Directory.EnumerateFiles(FilePaths.Roboto, "*.json")) {
			try {
				var jsonContent = await File.ReadAllTextAsync(filePath);
				var loadedState = JS.Deserialize<State>(jsonContent, JS.EnumConverter);

				Actors.Add(
					new ActorViewModel(
						actor: loadedState?.Options.Settings.Start.Feature.ToLowerInvariant() switch {
							"reddit" => new RedditActor { Options = loadedState.Options },
							_ => throw new NotSupportedException($"Feature '{loadedState?.Options.Settings.Start.Feature}' is not supported.")
						},
						selections: loadedState.Selections
				));
				Debug.WriteLine($"Loaded actor state from: {filePath}");
			} catch (Exception ex) {
				Debug.WriteLine($"Error loading actor state from {filePath}: {ex}");
				File.Delete(filePath);
			}
		}

		if (!Actors.Any(a => a.Actor is RedditActor)) {
			Debug.WriteLine("No saved Reddit actor found, adding default.");
			Actors.Add(new ActorViewModel(new RedditActor()));
		}
	}
}
