using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.Json.Serialization;

using DynamicData;
using DynamicData.Binding;

using Chameleon.client.MvvM;
using Chameleon.lib.Util;
using CommunityToolkit.Mvvm.ComponentModel;

using Chameleon.lib;
using Chameleon.lib.AIR.Actors;
using Chameleon.lib.AIR.Actors.Reddit;
using Chameleon.lib.Helpers;
using Chameleon.client.Features.Automation.Actors.Dialogs;
using Chameleon.client.Features.Projects.Profiles;
using Chameleon.lib.AIR.Scripts;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Playwright.Services;
using Chameleon.lib.Api.Dto;
using Chameleon.lib.Abs.Platformatic;
using ExCSS;
using Microsoft.AspNetCore.Http.Metadata;
using Chameleon.lib.Browzer;
namespace Chameleon.client.Features.Automation.Actors;

public static class Actorz {
	public record State(Opts Options, IEnumerable<Selection> Selections, IEnumerable<Tag> SelectedTags, IEnumerable<int> SelectedProfileIds);
	public static string CacheDir => FilePaths.EnsureDirectoryExists(
		FilePaths.AppDataDir, "Roboto"
	);
}

public partial class Tag(TagDto dto) : ObservableObject {
	[ObservableProperty] bool isSelected;
	public TagDto Dto { get; } = dto;

	[JsonIgnore]
	public IEnumerable<string> ProfileIds => Dto.Items
	.Where(x => x.Key == TagItemType.Profile)
	.SelectMany(x => x.Value);

	[JsonIgnore] public string ToolTipText => $"{ProfileIds.Count()} Profiles";
}

public partial class ActorViewModel : Automatior {
	[ObservableProperty] bool running;
	[ObservableProperty] string currentScriptTitle = string.Empty;

	public IActor Actor { get; }
	public ReadOnlyObservableCollection<Tag> Tagz { get; }
	public ReadOnlyObservableCollection<ObsProfile> SelectedProfiles { get; }

	public CompositeDisposable Subscriptions { get; } = [];
	public ObservableCollection<Selection> Selections { get; } = [];

	public lib.AIR.Actors.AI AI => Actor.Options.AI;
	public lib.AIR.Actors.Reddit.Args Args => Actor.Args as lib.AIR.Actors.Reddit.Args ??
		new lib.AIR.Actors.Reddit.Args(); // Ensure we have a valid Args instance TODO make more generic
	public lib.AIR.Actors.Settings Settings => Actor.Options.Settings; //new(new("x", 9, new(1, 1), new(1, 1)), new(30, 15, 60, new(256, 512)));

	public ActorViewModel(IActor actor) {
		Actor = actor;
		Actor.Scripts.Where(s => s is JSScript).Select(s => {
			return new Selection((JSScript)s);
		}).ForEach(Selections.Add);

		Subscriptions.Add(TagsRepo.Connect()
			.Filter(tag => tag.Items.Where(x => x.Key == TagItemType.Profile).Any())
			.Transform(item => new Tag(item))
			.Bind(out var tagz)
			.Subscribe());
		Tagz = tagz;

		Subscriptions.Add(ProfilesViewModel.Instance.ObsProfiles.ToObservableChangeSet()
			.AutoRefresh(profile => profile.IsSelected)
			.Filter(profile => profile.IsSelected)
			.Bind(out var profiles)
			.Subscribe());
		SelectedProfiles = profiles;

		Subscriptions.Add(Tagz.ToObservableChangeSet()
			.AutoRefresh(tag => tag.IsSelected)
			.ToCollection()
			.Subscribe(next => next.ForEach(t =>
				 ProfilesViewModel.Instance.ObsProfiles
				 .Where(x => t.ProfileIds.Contains(x.Dto.ID))
				 .ForEach(p => p.Active = p.IsSelected = t.IsSelected)
			)
		));

		CancellationTokenSource? cts = null;
		void onStop() {
			Running = false;
			CurrentScriptTitle = string.Empty;
			cts?.Cancel();
			cts?.Dispose();
			cts = null;
		}
		CancellationTokenSource onStart() {
			onStop();
			Running = true;
			cts = new CancellationTokenSource();
			return cts;
		}
		AsyncCommandMap["Play"] = async () => {
			try {
				var cts = onStart();
				var profiles = SelectedProfiles.Where(p => p.Active);
				if (!profiles.Any()) throw new Exception("No profiles selected to run.");

				var selected = Selections.OrderBy(s => new Random().Next()).Where(s => s.Selected).ToArray();
				if (selected.Length == 0) throw new Exception("No scripts selected to run.");

				var terms = Settings.Start.Terms.Split(',').Select(x => x.Trim()).Where(x => x.IsNot()).ToList();
				var urls = Settings.Start.Url?.Split('\n').Select(x => x.Trim()).Where(x => x.IsNot()).ToArray() ?? [];
				if (terms.Count == 0 && urls.Length == 0) throw new Exception("Search and URL's cannot be empty together.");
				else if (terms.Count != 0 && Settings.Start.Variations.Min > 0) terms.AddRange(
					await Service.I.Robo.Terms(new(AI.Decorators, Settings.Variations, terms)
				).WaitAsync(cts.Token));
				
				var termz = terms.OrderBy(s => new Random().Next(96)).ToArray();
				Actor.Options = Actor.Options with {
					Settings = Actor.Options.Settings with {
						Start = Actor.Options.Settings.Start with {
							Terms = string.Join(", ", termz)
						}
					}
				};
				OnPropertyChanged(nameof(Settings));
				int selectionIndex = 0, termsIndex = 0, urlsIndex = 0;
				await profiles.TryEach(async profile => {
					if (cts.IsCancellationRequested) return; // Check for cancellation before proceeding
					// Safer array access with proper bounds checking
					var selection = selected[selectionIndex++ % selected.Length];
					string[] urlser = urls.Length > 0 ? [urls[urlsIndex++ % urls.Length]] : [];
					string[] termer = termz.Length > 0 ? [termz[termsIndex++ % termz.Length]] : [];
					CurrentScriptTitle = selection.Script.Title + "...";
					Toaster.Info(
						$"Starting: '{CurrentScriptTitle}'",
						$"URL: {string.Join(", ", urlser)}",
						$"Search: {string.Join(", ", termer)}"
					);

					var browser = await profile.OpenBrowser(SelectedBrowserOption.Option, false).WaitAsync(cts.Token);
					await Run.Script(new() {
						Port = browser!.Settings.Profile.Port,
						Script = selection.Script,
						Opts = new Opts(
							AI, Args.ToDictionary(selected), Settings.ToRecord(urlser, termer, selection, new(0, 0))
						)
					}, cts.Token);

					Toaster.Info($"Finished: '{selection.Script.Title}'", $"Waiting '{Settings.Delay}'");
					await Task.Delay(TimeSpan.FromSeconds(Settings.Delay), cts.Token);
					
					if (Settings.CloseAfterRun) await browser.Closee().WaitAsync(cts.Token);
				});
			} finally {
				onStop();
			}
		};
		AsyncCommandMap["Edit"] = async () => {
			using var profileSelectorVM = new ProfileSelectorViewModel(SelectedProfiles);
			_ = await profileSelectorVM.ShowDialogAsync();
		};

		CommandMap["Stop"] = onStop;
	}

	public void LoadFromCache(IEnumerable<Selection>? selections, IEnumerable<string>? tags, IEnumerable<int>? profiles) {
		tags?.ForEach(id => {
			Tagz.Where(p => p.Dto.Name == id)
			.ForEach(p => p.IsSelected = true);
		});
		profiles?.ForEach(id => {
			ProfilesViewModel.Instance.ObsProfiles
			.Where(p => p.Dto.id == id)
			.ForEach(p => p.IsSelected = true);
		});
		Selections.Clear();
		selections?.ForEach(s => Selections.Add(new Selection(s.Script) {
			Selected = s.Selected
		}));

		Actor.Args = Args.Set(Actor.Options.Args);
		OnPropertyChanged(nameof(AI));
		OnPropertyChanged(nameof(Settings));
		OnPropertyChanged(nameof(Args));
	}

	public async Task Saverer() {
		Actor.Options.Settings.Start.Feature.ThrowIfNullOrEmpty();
		var currentArgs = Args.ToDictionary([]);
		var currentSettings = Settings.ToRecord();
		var currentOpts = new Opts(AI, currentArgs, currentSettings);
		var stateToSave = new Actorz.State(currentOpts, Selections, Tagz.Where(x => x.IsSelected), SelectedProfiles.Select(x => x.Dto.id));
		var filePath = Path.Combine(Actorz.CacheDir, $"{Actor.Options.Settings.Start.Feature}.json");
		var jsonContent = JSON.Serialize(stateToSave, JSON.EnumConverter);
		await File.WriteAllTextAsync(filePath, jsonContent);
		Toaster.Success("Saved");
	}
}

public partial class ActorsViewModel : OOVM {
	public static IEnumerable<BrowserOption> BrowserOptions { get; } = [new(BrowserType.Chrome), new(BrowserType.Brave)];

	[ObservableProperty] ActorViewModel selectedActor;
	public ObservableCollection<ActorViewModel> Actors { get; } = [new(new Reddit())];

	public ActorsViewModel() {
		SelectedActor = Actors[0];
		AsyncCommandMap["Save"] = async () => { await Actors.ForEach(a => a.Saverer()); };
	}

	private async Task LoadActorStates() {
		foreach (var filePath in Directory.EnumerateFiles(Actorz.CacheDir, "*.json")) {
			try {
				var jsonContent = await File.ReadAllTextAsync(filePath);
				var loadedState = JSON.Deserialize<Actorz.State>(jsonContent, JSON.EnumConverter);
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
				vm.LoadFromCache(
					selections: loadedState.Selections,
				 	tags: loadedState.SelectedTags.Select(x => x.Dto.Name),
					profiles: loadedState.SelectedProfileIds);
				Debug.WriteLine($"Loaded actor state from: {filePath}");
			} catch (Exception ex) {
				Toaster.Error($"Error loading actor state from {filePath}: {ex}");
				File.Delete(filePath);
			}
		}
	}
	public override async Task Init(object? param) {
		await base.Init(param);
		_ = await lib.Playwright.Project.Initialized.Task;
		if (!Loaded) await LoadActorStates();
	}

	public override async Task OnNavigatedTo(object? param) {
		await base.OnNavigatedTo(param);
	}

	public static ActorsViewModel Instance { get; } = new();
}
