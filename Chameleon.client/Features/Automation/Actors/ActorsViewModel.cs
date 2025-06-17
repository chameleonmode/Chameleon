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
	public static IEnumerable<BrowserOption> BrowserOptions { get; } = [new (SystemBrowserType.Chrome),new (SystemBrowserType.Brave) ];
	
	public ObservableCollection<ActorViewModel> Actors { get; } = [new (new RedditActor())];
	[ObservableProperty] ActorViewModel selectedActor;

	public ActorsViewModel() {
		SelectedActor = Actors[0];
		AsyncCommandMap["Save"] = async () => { await Actors.ForEach(a => a.Saverer()); };
	}

	private async Task LoadActorStates() {
		foreach (var filePath in Directory.EnumerateFiles(FilePaths.Roboto, "*.json")) {
			try {
				var jsonContent = await File.ReadAllTextAsync(filePath);
				var loadedState = JSON.Deserialize<State>(jsonContent, JSON.EnumConverter);
				ArgumentNullException.ThrowIfNull(loadedState, nameof(loadedState));

				var vm = loadedState.Options.Settings.Start.Feature.ToLowerInvariant() switch {
					"reddit" => Actors[0],
					_ => throw new NotSupportedException($"Feature '{loadedState?.Options.Settings.Start.Feature}' is not supported.")
				};
				vm.Actor.Options = new Opts(
					AI: loadedState.Options.AI ?? vm.Actor.Options.AI,
					Args: loadedState.Options.Args ?? vm.Actor.Options.Args,
					Settings: loadedState.Options.Settings ?? vm.Actor.Options.Settings
				);
				vm.LoadFromCache(selections: loadedState.Selections,
				 	tags: loadedState.SelectedTags.Select(x => x.Dto.Name),
					profiles: loadedState.SelectedProfileIds);
				vm.EditableSettings.Rando = vm.Actor.Options.Settings.Start.Rando.Min;
				Debug.WriteLine($"Loaded actor state from: {filePath}");
			} catch (Exception ex) {
				Debug.WriteLine($"Error loading actor state from {filePath}: {ex}");
				File.Delete(filePath);
			}
		}
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
