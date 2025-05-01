using System.Collections.ObjectModel;
using Chameleon.AIR.Actors.Models;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Text.Json;
using Chameleon.lib.CommunityToolkit.MvvM;
using RedditActor = Chameleon.AIR.Actors.Models.Reddit.Actor;
using Chameleon.lib.Const;

namespace Chameleon.client.Features.Automation.Actors;

public partial class ActorsViewModel : ViewModelObjectBase {
	public ObservableCollection<ActorViewModel> Actors { get; set; } = [];

	public ActorsViewModel() {
		LoadActorStates();
	}

	private static IActor CreateActorFromFeature(string featureName, Opts options) {

		return featureName.ToLowerInvariant() switch {
			"reddit" => new RedditActor { Options = options},
			_ => throw new NotSupportedException($"Feature '{featureName}' is not supported.")
		};
	}

	private void LoadActorStates() {
		Actors.Clear();

		if (!Directory.Exists(FilePaths.Roboto)) {
			Actors.Add(new ActorViewModel(new RedditActor()));
			return;
		}

		var jsonOptions = new JsonSerializerOptions {
			Converters = { new JsonStringEnumConverter() },
		};

		foreach (var filePath in Directory.EnumerateFiles(FilePaths.Roboto, "*.json")) {
			try {
				var jsonContent = File.ReadAllText(filePath);
				var loadedState = JsonSerializer.Deserialize<ActorState>(jsonContent, jsonOptions);

				if (loadedState != null && !string.IsNullOrWhiteSpace(loadedState.Options.Settings.Start.Feature)) {
					var loadedActor = CreateActorFromFeature(loadedState.Options.Settings.Start.Feature, loadedState.Options);
					if (loadedActor != null) {
						Actors.Add(new ActorViewModel(loadedActor, loadedState.SelectedScriptFiles));
						Debug.WriteLine($"Loaded actor state from: {filePath}");
					}
				} else {
					Debug.WriteLine($"Failed to load valid ActorState or Feature name from: {filePath}");
				}
			} catch (Exception ex) {
				Debug.WriteLine($"Error loading actor state from {filePath}: {ex}");
			}
		}

		if (!Actors.Any(a => a.Actor is RedditActor)) {
			Debug.WriteLine("No saved Reddit actor found, adding default.");
			Actors.Add(new ActorViewModel(new RedditActor()));
		}
	}
}
