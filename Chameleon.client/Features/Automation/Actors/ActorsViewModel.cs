using System.Collections.ObjectModel;
using System.Diagnostics;
using Chameleon.client.MvvM;
using RedditActor = Chameleon.AIR.Actors.Models.Reddit.Actor;
using Chameleon.AIR.Actors.Models;
using Chameleon.lib.Util;
using CommunityToolkit.Mvvm.ComponentModel;

using Chameleon.lib;
using Chameleon.lib.WebBrowser;

namespace Chameleon.client.Features.Automation.Actors;

public partial class ActorsViewModel : ViewModelObjectBase {
	[ObservableProperty] BrowserOption selectedBrowserOption;
	[ObservableProperty] ActorViewModel? selectedActor;

	public IEnumerable<BrowserOption> BrowserOptions { get; } = [
		new (SystemBrowserType.Chrome),
		new (SystemBrowserType.Brave),
	];
	public ObservableCollection<ActorViewModel> Actors { get; set; } = [];

	public ActorsViewModel() {
		SelectedBrowserOption = BrowserOptions.First();
	 }

	private async Task LoadActorStates() {
		Actors.Clear();

		foreach (var filePath in Directory.EnumerateFiles(FilePaths.Roboto, "*.json")) {
			try {
				var jsonContent = await File.ReadAllTextAsync(filePath);
				var loadedState = JS.Deserialize<State>(jsonContent, JS.EnumConverter);
				ArgumentNullException.ThrowIfNull(loadedState, nameof(loadedState));

				var actor = loadedState.Options.Settings.Start.Feature.ToLowerInvariant() switch {
					"reddit" => new RedditActor(),
					_ => throw new NotSupportedException($"Feature '{loadedState?.Options.Settings.Start.Feature}' is not supported.")
				};
				actor.Options = new Opts(
					AI: loadedState.Options.AI ?? actor.Options.AI,
					Args: loadedState.Options.Args ?? actor.Options.Args,
					Settings: loadedState.Options.Settings ?? actor.Options.Settings
				);

				var vm = new ActorViewModel(
					actor,
				 	selections: loadedState.Selections,
				 	selectedTags: loadedState.SelectedTags.Select(x => x.Dto.Name),
					profileSelections: loadedState.SelectedProfileIds
				);
				vm.EditableSettings.Rando = actor.Options.Settings.Start.Rando.Min;
				Actors.Add(vm);
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
		SelectedActor = Actors[0];
	}
	public override async Task InitAsync(object? param) {
		await base.InitAsync(param);
		_ = await lib.Playwright.Project.Initialized.Task;
		if (!Loaded) await LoadActorStates();
	}

	public override async Task OnNavigatedToAsync(object? param) {
		await base.OnNavigatedToAsync(param);
	}

	public static ActorsViewModel Instance { get; } = new();
}
