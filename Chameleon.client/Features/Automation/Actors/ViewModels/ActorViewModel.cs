using Chameleon.client.Features.Automation.Actors.Dialogs;
using Chameleon.client.Features.Automation.Actors.ViewModels;
using Chameleon.client.Features.Projects.Profiles;
using Chameleon.lib.AIR.Scripts;
using Chameleon.lib.Api.Repos;
using Chameleon.client.MvvM;
using Chameleon.lib.Helpers;
using Chameleon.lib.Playwright.Services;
using Chameleon.lib.Util;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using DynamicData.Binding;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.Json.Serialization;
using Chameleon.lib;
using Chameleon.lib.Api.Dto;
using Chameleon.lib.WebBrowser;
using Chameleon.lib.Abs.Platformatic;
using Chameleon.lib.AIR.Actors;
namespace Chameleon.client.Features.Automation.Actors;

public partial class Tag(TagDto dto) : ObservableObject {
	public TagDto Dto { get; } = dto;
	[ObservableProperty] bool isSelected;

	[JsonIgnore] public IEnumerable<string> ProfileIds => Dto.Items
	.Where(x => x.Key == TagItemType.Profile)
	.SelectMany(x => x.Value);

	[JsonIgnore] public string ToolTipText => $"{ProfileIds.Count()} Profiles";
}
public record State(Opts Options, IEnumerable<Selection> Selections, IEnumerable<Tag> SelectedTags, IEnumerable<int> SelectedProfileIds);

public partial class ActorViewModel : ViewModelObjectBase {
	CancellationTokenSource? cts;
	[ObservableProperty] bool running;
	[ObservableProperty] BrowserOption selectedBrowserOption;

	public IActor Actor { get; }
	public ReadOnlyObservableCollection<Tag> Tagz { get; }
	public ReadOnlyObservableCollection<ObsProfile> SelectedProfiles { get; }

	public CompositeDisposable Subscriptions { get; } = [];
	public ObservableCollection<Selection> Selections { get; } = [];
	public ArgsViewModel EditableArgs { get; } = new();
	public lib.AIR.Actors.AI AISettings => Actor.Options.AI;
	public lib.AIR.Actors.Settings EditableSettings => Actor.Options.Settings; //new(new("x", 9, new(1, 1), new(1, 1)), new(30, 15, 60, new(256, 512)));

	public ActorViewModel(IActor actor) {
		Actor = actor;
		SelectedBrowserOption = ActorsViewModel.BrowserOptions.First();

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
		AsyncCommandMap["OpenProfileSelector"] = async () => {
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
		Actor.Scripts.Where(s => s is Script).Select(s => {
			var selected = selections?.FirstOrDefault(x => x.Script.Title == s.Title)?.Selected ?? false;
			return new Selection((Script)s, selected);
		}).ForEach(Selections.Add);
		EditableArgs.Set(Actor.Options.Args);
		OnPropertyChanged(nameof(AISettings));
		OnPropertyChanged(nameof(EditableSettings));
		OnPropertyChanged(nameof(EditableArgs));
	}

	public async Task Saverer() {
		Actor.Options.Settings.Start.Feature.ThrowIfNullOrEmpty();
		var currentArgs = EditableArgs.ToDictionary([], EditableArgs.Search.Split(','));
		var currentSettings = EditableSettings.ToRecord();
		var currentOpts = new Opts(AISettings, currentArgs, currentSettings);
		var stateToSave = new State(currentOpts, Selections, Tagz.Where(x => x.IsSelected), SelectedProfiles.Select(x => x.Dto.id));
		var filePath = Path.Combine(FilePaths.Roboto, $"{Actor.Options.Settings.Start.Feature}.json");
		var jsonContent = JSON.Serialize(stateToSave, JSON.EnumConverter);
		await File.WriteAllTextAsync(filePath, jsonContent, cts?.Token ?? CancellationToken.None);
		Toaster.Success("Saved");
	}

	public async Task Runerer() {
		var presearch = EditableArgs.Search;
		Running = true;
		cts = new();
		try {
			var profiles = SelectedProfiles.Where(p => p.Active);
			if (!profiles.Any()) throw new Exception("No profiles selected to run.");

			var selected = Selections.OrderBy(s => new Random().Next()).Where(s => s.Selected);
			if (!selected.Any()) throw new Exception("No scripts selected to run.");

			var terms = EditableArgs.Search.Split(',').Where(x => x.IsNot()).Select(x => x.Trim()).ToList();
			var urls = EditableSettings.Start.Url?.Split('\n').Where(x => x.IsNot()).Select(x => x.Trim()).ToList() ?? [];
			if (terms.Count == 0 && urls.Count == 0) throw new Exception("Search and URL's cannot be empty together.");
			else if (terms.Count != 0 && EditableSettings.Start?.Variations.Min > 0) {
				Toaster.Info($"Generating {EditableSettings.Start.Variations.Min} term(s) for each search term");
				var res = await Service.Routes.Promptee.Genorate(new(
					AISettings.Decorators,
					EditableSettings.Start.Variations.Min,
					terms
				)).WaitAsync(cts.Token);
				terms.AddRange(res!.Reply.SelectMany(i => {
					//var termy = i.Data.Split(',').Select(t => t.Trim()).Where(t => t.IsNot());
					//return termy;
					return i.Data.Select(t => t.Trim()).Where(t => t.IsNot());
				}) ?? []);
				for (var i = 0; i < terms.Count; i++) {
					terms = [.. terms.OrderBy(s => new Random().Next())];
				}
				EditableArgs.Search = string.Join(", ", terms);
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
					Port = browser!.Settings.Port,
					Script = selection.Script,
					Opts = new Opts(
					AISettings,
					EditableArgs.ToDictionary(selected, termer),
					EditableSettings.ToRecord(urlser, selection.Script.Title == "Surf" ? new(0, 0) : null, new(0, 0)))
				}, cts.Token);
				Toaster.Info($"Finished: '{selection.Script.Title}'", $"Waitnig '{EditableSettings.Delay}'");

				await Task.Delay(TimeSpan.FromSeconds(EditableSettings.Delay), cts.Token);
				if (EditableSettings.CloseAfterRun) {
					await ProcessUtil.TryKillProcess(browser.Brocess);
					browser.Close();
				}
			});

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
			EditableArgs.Search = presearch;
		}
	}

	private void Stoperer() {
		cts?.Cancel();
		cts?.Dispose();
		cts = null;
		Running = false;
	}
}