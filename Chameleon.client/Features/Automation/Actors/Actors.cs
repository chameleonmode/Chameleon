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
using Chameleon.lib.WebBrowser;
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
namespace Chameleon.client.Features.Automation.Actors;

public record State(Opts Options, IEnumerable<Selection> Selections, IEnumerable<Tag> SelectedTags, IEnumerable<int> SelectedProfileIds);

public partial class Tag(TagDto dto) : ObservableObject {
	[ObservableProperty] bool isSelected;
	public TagDto Dto { get; } = dto;

	[JsonIgnore]
	public IEnumerable<string> ProfileIds => Dto.Items
	.Where(x => x.Key == TagItemType.Profile)
	.SelectMany(x => x.Value);

	[JsonIgnore] public string ToolTipText => $"{ProfileIds.Count()} Profiles";
}

public partial class ActorViewModel : ViewModelObjectBase {
	CancellationTokenSource? cts;
	[ObservableProperty] bool running;
	[ObservableProperty] BrowserOption selectedBrowserOption = ActorsViewModel.BrowserOptions.First();

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
			.DistinctUntilChanged()
			.Bind(out var selectedProfiles)
			.Subscribe());
		SelectedProfiles = selectedProfiles;

		Subscriptions.Add(Tagz.ToObservableChangeSet()
			.AutoRefresh(tag => tag.IsSelected).ToCollection()
			.Subscribe(next =>
				next.ForEach(t =>
					 ProfilesViewModel.Instance.ObsProfiles
					.Where(x => t.ProfileIds.Contains(x.Dto.ID))
					.ForEach(p => p.Active = p.IsSelected = t.IsSelected)
			)));

		AsyncCommandMap["Play"] = Runerer;
		AsyncCommandMap["Edit"] = async () => {
			using var profileSelectorVM = new ProfileSelectorViewModel(SelectedProfiles);
			_ = await profileSelectorVM.ShowDialogAsync();
		};

		CommandMap["Stop"] = Stoperer;
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
		var stateToSave = new State(currentOpts, Selections, Tagz.Where(x => x.IsSelected), SelectedProfiles.Select(x => x.Dto.id));
		var filePath = Path.Combine(FilePaths.Roboto, $"{Actor.Options.Settings.Start.Feature}.json");
		var jsonContent = JSON.Serialize(stateToSave, JSON.EnumConverter);
		await File.WriteAllTextAsync(filePath, jsonContent, cts?.Token ?? CancellationToken.None);
		Toaster.Success("Saved");
	}

	public async Task Runerer() {
		var presearch = Settings.Start.Terms;
		Running = true;
		cts = new();
		try {
			var profiles = SelectedProfiles.Where(p => p.Active);
			if (!profiles.Any()) throw new Exception("No profiles selected to run.");

			var selected = Selections.OrderBy(s => new Random().Next()).Where(s => s.Selected);
			if (!selected.Any()) throw new Exception("No scripts selected to run.");

			var terms = Settings.Start.Terms.Split(',').Where(x => x.IsNot()).Select(x => x.Trim()).ToList();
			var urls = Settings.Start.Url?.Split('\n').Where(x => x.IsNot()).Select(x => x.Trim()).ToList() ?? [];
			if (terms.Count == 0 && urls.Count == 0) throw new Exception("Search and URL's cannot be empty together.");
			else if (terms.Count != 0 && Settings.Start?.Variations.Min > 0) {
				Toaster.Info($"Generating {Settings.Start.Variations.Min} term(s) for each search term");
				var generated = await Service.Routes.Promptee.Genorate(
					new(AI.Decorators, Settings.Variations, terms)
				).WaitAsync(cts.Token);

				terms.AddRange(generated!.Reply.SelectMany(i => i.Data.Select(t => t.Trim()).Where(t => t.IsNot())));
				do {
					var zoro = terms[0];
					terms = [.. terms.OrderBy(s => new Random().Next(18))];
					if (terms[0] != zoro) break; // If the first term is not the same as the original, we are done
				} while (!cts.IsCancellationRequested);
				Actor.Options = Actor.Options with {
					Settings = Actor.Options.Settings with {
						Start = Actor.Options.Settings.Start with {
							Terms = string.Join(", ", terms)
						}
					}
				};
				OnPropertyChanged(nameof(Settings));
			}
			int selectionIndex = -1, termsIndex = -1, urlsIndex = -1;

			await profiles.ForEach(async profile => {
				var selection = selected.ElementAt(++selectionIndex >= selected.Count() ? selectionIndex = 0 : selectionIndex);

				string[] urlser = ++urlsIndex >= urls.Count ? [] : [urls[urlsIndex]];
				string[] termer = terms.Count == 0 ? [] : [terms[++termsIndex >= terms.Count ? termsIndex = 0 : termsIndex]];
				Toaster.Info(
					$"Starting: '{selection.Script.Title}'",
					$"Using URL: {string.Join(", ", urlser)}",
					$"Using term: {string.Join(", ", termer)}");

				var browser = await profile.OpenSystemBrowser(SelectedBrowserOption.Option, false).WaitAsync(cts.Token);
				await Run.Script(new() {
					Port = browser!.Settings.Profile.Port,
					Script = selection.Script,
					Opts = new Opts(AI, Args.ToDictionary(selected), Settings.ToRecord(urlser, termer, selection, new(0, 0)))
				}, cts.Token);
				Toaster.Info($"Finished: '{selection.Script.Title}'", $"Waitnig '{Settings.Delay}'");

				await Task.Delay(TimeSpan.FromSeconds(Settings.Delay), cts.Token);
				if (Settings.CloseAfterRun) await browser.Closee().WaitAsync(cts.Token);
			}, cts.Token);

			// TODO:
			// if (EditableSettings.EachProfile) foreach (var selection in selected) {
			// 		foreach (var profile in profiles) {
			// 			var browser = await ExecuteScriptAsync(selection, profile);
			// 			await BrowserShutdown(browser);
			// 		}
			// 	}
			// else foreach (var profile in profiles) {
			// 		IBrowserInstance? browser = null;
			// 		foreach (var selection in EditableSettings.AsQue
			// 		? [selected.ElementAt(selectionIndex++ >= selected.Count() ? selectionIndex = 0 : selectionIndex)] : selected) {
			// 			browser = await ExecuteScriptAsync(selection, profile);
			// 			if (EditableSettings.AsQue) await BrowserShutdown(browser);
			// 		}
			// 		if (!EditableSettings.AsQue) await BrowserShutdown(browser);
			// 	}
			// foreach (var profile in profiles) {
			// 	if (++selectionIndex >= selected.Count()) selectionIndex = 0;
			// 	var selection = selected.ElementAt(selectionIndex);
			// 	await ExecuteScriptAsync(selection, profile);
			// }
		} finally {
			Stoperer();
			Settings.Start.Terms = presearch;
		}
	}

	private void Stoperer() {
		cts?.Cancel();
		cts?.Dispose();
		cts = null;
		Running = false;
	}
}

public partial class ActorsViewModel : ViewModelObjectBase {
	public static IEnumerable<BrowserOption> BrowserOptions { get; } = [new(SystemBrowserType.Chrome), new(SystemBrowserType.Brave)];

	[ObservableProperty] ActorViewModel selectedActor;
	public ObservableCollection<ActorViewModel> Actors { get; } = [new(new Reddit())];

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
