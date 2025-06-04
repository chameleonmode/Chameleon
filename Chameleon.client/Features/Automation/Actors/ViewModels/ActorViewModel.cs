using Chameleon.AIR.Actors.Models;
using Chameleon.client.Features.Automation.Actors.Dialogs;
using Chameleon.client.Features.Automation.Actors.ViewModels;
using Chameleon.client.Features.Projects.Profiles;
using Chameleon.lib.AIR.Scripts.Models;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Const;
using Chameleon.lib.Playwright.Services;
using Chameleon.lib.Util;
using Chameleon.lib.WebBrowser;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using DynamicData.Binding;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.Json.Serialization;
using static Chameleon.lib.Common.Constants.Enums;
namespace Chameleon.client.Features.Automation.Actors;

public partial class Tag(TagDto dto, bool isSelected)  : ObservableObject {
	[ObservableProperty] bool isSelected = isSelected;
	public TagDto Dto { get; } = dto;

	[JsonIgnore] public IEnumerable<string> ProfileIds => Dto.Items
	.Where(x => x.Key == TagItemType.Profile)
	.SelectMany(x => x.Value);

	[JsonIgnore] public string ToolTipText => $"{ProfileIds.Count()} Profiles";
}
public record Selection(Script Script, bool Selected = false);
public record State(Opts Options, IEnumerable<Selection> Selections, IEnumerable<Tag> SelectedTags, IEnumerable<int> SelectedProfileIds);
public record BrowserOption(SystemBrowserType Option) {
	public string IconName { get; } = Option.ToString().ToLower();
}

public partial class ActorViewModel : ViewModelObjectBase {
	readonly CompositeDisposable subscriptions = [];

	CancellationTokenSource? cts;

	[ObservableProperty] bool running;
	[ObservableProperty] AIR.Actors.Models.AI aiSettings;
	[ObservableProperty] ArgsViewModel editableArgs;
	[ObservableProperty] SettingsViewModel editableSettings;
	[ObservableProperty] BrowserOption browser;

	public IActor Actor { get; }
	public List<Selection> Selections { get; }
	public IEnumerable<BrowserOption> BrowserOptions { get; } = [
		new (SystemBrowserType.Chrome),
		new (SystemBrowserType.Brave),
	];

	public ReadOnlyObservableCollection<Tag> Tagz { get; }
	public ReadOnlyObservableCollection<ObsProfile> SelectedProfiles { get; }

	public ActorViewModel(
		IActor actor,
		IEnumerable<Selection>? selections = null,
		IEnumerable<string>? selectedTags = null,
		IEnumerable<int>? profileSelections = null
	) {
		subscriptions.Add(
			TagsRepo.Connect()
			.Filter(tag => tag.Items.Where(x => x.Key == TagItemType.Profile).Any())
			.Transform(item => new Tag(item, selectedTags?.Contains(item.Name) ?? false))
			.Bind(out var tagz)
			.Subscribe());
		Tagz = tagz;

		subscriptions.Add(
			 ProfilesViewModel.Instance.Profiles.ToObservableChangeSet()
			.AutoRefresh(profile => profile.IsSelected)
			.Filter(profile => profile.IsSelected)
			.Sort(SortExpressionComparer<ObsProfile>.Ascending(p => p.Title ?? ""))
			.DistinctUntilChanged()
			.Bind(out var selectedProfiles)
			.Subscribe());
		SelectedProfiles = selectedProfiles;

		subscriptions.Add(
			tagz.ToObservableChangeSet()
			.AutoRefresh(tag => tag.IsSelected)
			.ToCollection()
			.Subscribe(next =>
			next.ForEach(t =>
					 ProfilesViewModel.Instance.Profiles
					.Where(x => t.ProfileIds.Contains(x.Dto.ID))
					.ForEach(p => p.Active = p.IsSelected = t.IsSelected)
				)));

		Actor = actor;
		AiSettings = actor.Options.AI;
		Selections = [.. actor.Scripts.Select(s => {
			if (s is not Script script) return null;
				var selected = selections?.FirstOrDefault(x => x.Script.File == script.File)?.Selected ?? false;
				return new Selection(script, selected);
			}).Where(s => s != null)
		];

		profileSelections?.ForEach(id => {
			 ProfilesViewModel.Instance.Profiles
			.Where(p => p.Dto.id == id)
			.ForEach(p => p.IsSelected = true);
		});

		EditableArgs = new(actor.Options.Args);
		EditableSettings = new(actor.Options.Settings);
		Browser = BrowserOptions.First();

		AsyncCommandMap["Run"] = async () => {
			try {
				var profiles = SelectedProfiles.Where(p => p.Active);
				if (!profiles.Any()) throw new Exception("No profiles selected to run.");

				var selected = Selections.OrderBy(s => new Random().Next()).Where(s => s.Selected);
				if (!selected.Any()) throw new Exception("No scripts selected to run.");

				var things = EditableArgs.Search.Is() && EditableSettings.Start.Url.Is();
				if (things) throw new Exception("Search and URL's cannot be empty together.");

				Running = true;
				cts = new CancellationTokenSource();

				if (EditableSettings.Start.ExecuteOneScriptAccrosProfiles) {
					await RunAllProfilesPerScriptAsync(profiles, selected);
				} else {
					await RunAllScriptsPerProfileAsync(profiles, selected);
				}

			} finally {
				if (Running) CommandMap["Stop"]();
				await AsyncCommandMap["Save"]();
			}
		};
		AsyncCommandMap["Save"] = async () => {
			Actor.Options.Settings.Start.Feature.ThrowIfNullOrEmpty();
			var currentArgs = EditableArgs.ToDictionary([]);
			var currentSettings = EditableSettings.ToRecord();
			var currentOpts = new Opts(AiSettings, currentArgs, currentSettings);
			var stateToSave = new State(currentOpts, Selections, Tagz.Where(x => x.IsSelected), SelectedProfiles.Select(x => x.Dto.id));
			var filePath = Path.Combine(FilePaths.Roboto, $"{Actor.Options.Settings.Start.Feature}.json");
			var jsonContent = JS.Serialize(stateToSave, JS.EnumConverter);
			await File.WriteAllTextAsync(filePath, jsonContent, cts?.Token ?? CancellationToken.None);
		};
		AsyncCommandMap["OpenProfileSelector"] = async () => {
			using var profileSelectorVM = new ProfileSelectorViewModel(SelectedProfiles);
			_ = await profileSelectorVM.ShowDialogAsync();
		};

		CommandMap["Stop"] = () => {
			if (cts != null) {
				cts.Cancel();
				cts.Dispose();
				cts = null;
			}
			Running = false;
		};
	}

	private async Task RunAllScriptsPerProfileAsync(IEnumerable<ObsProfile> profiles, IEnumerable<Selection> selected) {
		foreach (var profile in profiles) {
			cts!.Token.ThrowIfCancellationRequested();

			var browser = await profile.OpenSystemBrowser(Browser.Option).WaitAsync(cts.Token);
			ArgumentNullException.ThrowIfNull(browser);

			var opts = new Opts(AiSettings, EditableArgs.ToDictionary(selected), EditableSettings.ToRecord());
			foreach (var selection in selected) {
				await ExecuteScriptAsync(selection, opts, profile, browser);
			}
		}
	}

	private async Task RunAllProfilesPerScriptAsync(IEnumerable<ObsProfile> profiles, IEnumerable<Selection> selected) {
		foreach (var selection in selected) {
			cts!.Token.ThrowIfCancellationRequested();

			Debug.WriteLine($"Starting Script '{selection.Script.Title}' across all profiles");
			var opts = new Opts(AiSettings, EditableArgs.ToDictionary(selected), EditableSettings.ToRecord());

			foreach (var profile in profiles) {
				cts.Token.ThrowIfCancellationRequested();

				var browser = await profile.OpenSystemBrowser(Browser.Option).WaitAsync(cts.Token);
				ArgumentNullException.ThrowIfNull(browser);

				await ExecuteScriptAsync(selection, opts, profile, browser);
			}

			Debug.WriteLine($"Finished Script '{selection.Script.Title}'");
			await Task.Delay(TimeSpan.FromSeconds(EditableSettings.RandomWaitPerProfile), cts.Token);
		}
	}

	private async Task ExecuteScriptAsync(Selection selection, Opts opts, ObsProfile profile, IBrowserInstance? browser) {
		try {
			var json = JS.Serialize(opts);
			Debug.WriteLine($@"Running script with:
                                 Profile '{profile.Title}', Script '{selection.Script.Title}' with Feature '{opts.Settings.Start.Feature}'
                            ");
			Debug.WriteLine($@"Opts: {json}");

			await Run.Script(new() {
				Port = browser!.Settings.Port,
				Script = selection.Script,
				Opts = opts
			}, cts!.Token);

		} finally {
			if (EditableSettings.Start.CloseOldBrowserProfileAfterRun)
				browser!.Close();
		}
	}
}