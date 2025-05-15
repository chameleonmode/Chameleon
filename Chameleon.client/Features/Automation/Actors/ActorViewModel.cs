using Chameleon.AIR.Actors.Models;
using Chameleon.AIR.Scripts.Models;
using Chameleon.app.Avalonia.Features.ProfilesAndFolders.Folders;
using Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.MyProfiles;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.client.Features.Automation.Actors.Dialogs;
using Chameleon.client.Features.Automation.Actors.ViewModels;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Const;
using Chameleon.lib.Playwright.Utils;
using Chameleon.lib.Util;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using DynamicData.Binding;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using static Chameleon.lib.Common.Constants.Enums;
namespace Chameleon.client.Features.Automation.Actors;
// multiple storage state / results
// more ai stuffs

public partial class Tag : ObservableObject
{
	[ObservableProperty] bool isSelected;

	public TagDto Dto { get; }

	public IEnumerable<string> ProfileIds => Dto.Items
																						 .Where(x => x.Key == TagItemType.Profile)
																						 .SelectMany(x => x.Value);

	public string ToolTipText => $"{ProfileIds.Count()} Profiles";

	public Tag(TagDto dto, bool isSelected = false) {
		Dto = dto;
		IsSelected = isSelected;
	}
}
public record Selection(Script Script, bool Selected = false);
public record State(Opts Options, IEnumerable<Selection> Selections, IEnumerable<Tag> SelectedTags, IEnumerable<int> SelectedProfileIds);
public record BrowserOption(SystemBrowserType Option) {
	public string IconName { get; } = Option.ToString().ToLower();
}

public partial class ActorViewModel : ViewModelObjectBase {
	CancellationTokenSource? cts;
	private static readonly Random random = new();
	private readonly CompositeDisposable subscriptions = [];
	private readonly HashSet<int> initialSelectedProfileIdsHashSet;
	private readonly HashSet<string> initialSelectedTagNamesHashSet;

	[ObservableProperty] bool running;
	[ObservableProperty] AIR.Actors.Models.AI aiSettings;
	[ObservableProperty] ArgsViewModel editableArgs;
	[ObservableProperty] SettingsViewModel editableSettings;
	[ObservableProperty] BrowserOption browser;

	public IEnumerable<BrowserOption> BrowserOptions { get; } = [
		new (SystemBrowserType.Chrome),
		new (SystemBrowserType.Brave),
	];

	public IActor Actor { get; }
	public List<Selection> Selections { get; }

	private readonly ReadOnlyObservableCollection<Tag> tagz;
	public ReadOnlyObservableCollection<Tag> Tagz => tagz;

	private ReadOnlyObservableCollection<ObsFolder> Folders => FoldersViewModel.Instance.Folders;

	private ReadOnlyObservableCollection<ObsProfile> Profiles => MyProfilesViewModel.Instance.Profiles;

	private readonly ReadOnlyObservableCollection<ObsProfile> selectedProfiles;
	public ReadOnlyObservableCollection<ObsProfile> SelectedProfiles => selectedProfiles;

	public ActorViewModel(IActor actor, IEnumerable<Selection>? selections = null,
		IEnumerable<string>? initialSelectedTagNames = null,
		IEnumerable<int>? initialSelectedProfileIds = null) {

		initialSelectedProfileIdsHashSet = new HashSet<int>(initialSelectedProfileIds ?? []);
		initialSelectedTagNamesHashSet = new HashSet<string>(initialSelectedTagNames ?? []);

		var profilesSortExpression = SortExpressionComparer<ObsProfile>.Ascending(p => p.Title ?? "");
		var selectionUpdater = Profiles.ToObservableChangeSet()
				.AutoRefresh(profile => profile.IsSelected)
				.Filter(profile => profile.IsSelected)
				.Sort(profilesSortExpression)
				.DistinctUntilChanged()
				.Bind(out selectedProfiles)
				.Subscribe();
		subscriptions.Add(selectionUpdater);

		var tagzSource = TagsRepo.Connect()
		.Filter(tag => tag.Items.Where(x => x.Key == TagItemType.Profile).Any())
		.Transform(item => new Tag(item, initialSelectedTagNamesHashSet?.Contains(item.Name) ?? false))
		.Bind(out tagz)
		.Subscribe();
		subscriptions.Add(tagzSource);

		var tagSelectionSynchronizer = tagz.ToObservableChangeSet()
						.AutoRefresh(tag => tag.IsSelected)
						.ToCollection()
						.Subscribe(currentTags => {
							var profileIdsToSelect = currentTags
									.Where(tag => tag.IsSelected)
									.SelectMany(tag => tag.ProfileIds)
									.ToHashSet();

							foreach (var profile in Profiles) {
								if (profile.Dto?.id != null)
									profile.IsSelected = profileIdsToSelect.Contains(profile.Dto.id.ToString());
							}
						},
						ex => Debug.WriteLine($"Error in Tag selection synchronizer: {ex}")
						);
		subscriptions.Add(tagSelectionSynchronizer);

		Actor = actor;
		AiSettings = actor.Options.AI;
		Selections = [.. actor.Scripts.Select(s =>{
				if (s is not Script script) return null;
				var selected = selections?.FirstOrDefault(x => x.Script.File == script.File)?.Selected ?? false;
				return new Selection(script, selected);
			}).Where(s => s != null)
		];

		InitializeSelectedProfiles();

		EditableArgs = new(actor.Options.Args);
		EditableSettings = new(actor.Options.Settings);
		Browser = BrowserOptions.First();

		AsyncCommandMap["Run"] = async () => {
			try {

				if (SelectedProfiles == null || SelectedProfiles.Count == 0) {
					Debug.WriteLine("No profiles selected from tags or 'Select..' button. Prompting with main selector.");
					await AsyncCommandMap["OpenProfileSelector"].Invoke();

					if (SelectedProfiles == null || SelectedProfiles.Count == 0)
						throw new OperationCanceledException("No profiles selected for the run after all attempts.");
				}

				var selected = await EnsureScriptsSelectedAsync();
				if (EditableArgs.Search.Is() && EditableSettings.Start.Url.Is()) throw new Exception("Search and URL's cannot be empty together.");

				Running = true;
				cts = new CancellationTokenSource();
				foreach (var profile in SelectedProfiles) {
					cts.Token.ThrowIfCancellationRequested();

					var browser = await profile.OpenSystemBrowser(Browser.Option).WaitAsync(cts.Token);
					ArgumentNullException.ThrowIfNull(browser);

					var shuffledScripts = selected.OrderBy(s => random.Next());
					foreach (var selection in shuffledScripts) {
						cts.Token.ThrowIfCancellationRequested();

						var opts = new Opts(AiSettings, EditableArgs.ToDictionary(), EditableSettings.ToRecord());
						var json = JS.Serialize(opts);
						Debug.WriteLine($@"Running script with:
							  Profile '{profile.Title}', Script '{selection.Script.Title}' with Feature '{opts.Settings.Start.Feature}'");
						Debug.WriteLine(json);

						await PlaywriteRunner.RunScript(new() {
							Port = browser.Settings.Port,
							Script = selection.Script,
							Opts = opts
						}, cts.Token);
					}
				}
			} finally {
				if (Running) CommandMap["Stop"]();
				await AsyncCommandMap["Save"]();
			}
		};
		AsyncCommandMap["Save"] = async () => {
			Actor.Options.Settings.Start.Feature.ThrowIfNullOrEmpty();
			var currentArgs = EditableArgs.ToDictionary();
			var currentSettings = EditableSettings.ToRecord();
			var currentOpts = new Opts(AiSettings, currentArgs, currentSettings);
			var stateToSave = new State(currentOpts, Selections, Tagz.Where(x => x.IsSelected), SelectedProfiles.Select(x => x.Dto.id));
			var filePath = Path.Combine(FilePaths.Roboto, $"{Actor.Options.Settings.Start.Feature}.json");
			var jsonContent = JS.Serialize(stateToSave, JS.EnumConverter);
			await File.WriteAllTextAsync(filePath, jsonContent, cts?.Token ?? CancellationToken.None);
		};

		CommandMap["Stop"] = () => {
			if (cts != null) {
				cts.Cancel();
				cts.Dispose();
				cts = null;
			}
			Running = false;
		};

		AsyncCommandMap["OpenProfileSelector"] = async () => {
			using var profileSelectorVM = new ProfileSelectorViewModel(Folders, Profiles, SelectedProfiles);
			_ = await profileSelectorVM.ShowDialogAsync();
		};
	}

	private void InitializeSelectedProfiles() => 
		_ = Observable.Timer(TimeSpan.FromMilliseconds(150))
				.Subscribe(_ => {
					foreach (var profile in Profiles) {
						if (profile.Dto?.id != null && initialSelectedProfileIdsHashSet.Contains(profile.Dto.id)) {
							profile.IsSelected = true;
						}
					}
				});
	async Task<List<Selection>> EnsureScriptsSelectedAsync() {
		var selectedScripts = Selections.Where(s => s.Selected).ToList();
		if (selectedScripts.Count == 0) {
			var scriptSelectorVM = new ScriptsSelectorViewModel(this.Selections);
			var scriptsChosen = await scriptSelectorVM.ShowDialogAsync();
			if (!scriptsChosen)
				throw new OperationCanceledException("Script selection cancelled or none chosen");
			selectedScripts = this.Selections.Where(s => s.Selected).ToList();
			if (selectedScripts.Count == 0)
				throw new InvalidOperationException("Dialog returned OK but no scripts were selected");
		}
		return selectedScripts;
	}

	private bool disposed = false;
	public void Dispose() {
		if (disposed) return;
		subscriptions.Dispose();
		disposed = true;
	}
}