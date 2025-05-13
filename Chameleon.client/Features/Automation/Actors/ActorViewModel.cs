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
using Chameleon.lib.Helpers;
using Chameleon.lib.Playwright.Utils;
using Chameleon.lib.Util;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using System.Collections.ObjectModel;
using System.Diagnostics;
using static Chameleon.lib.Common.Constants.Enums;
namespace Chameleon.client.Features.Automation.Actors;
// multiple storage state / results
// more ai stuffs
public record Tag(TagDto Dto, bool Selected = false) {
	public IEnumerable<string> ProfileIds => Dto.Items
																						 .Where(x => x.Key == TagItemType.Profile)
																						 .SelectMany(x => x.Value);
	public string ToolTipText => $"{ProfileIds.Count()} Profiles";
}
public record Selection(Script Script, bool Selected = false);
public record State(Opts Options, IEnumerable<Selection> Selections, IEnumerable<Tag> SelectedTags, IEnumerable<int> SelectedProfileIds);
public record BrowserOption(SystemBrowserType Option) {
	public string IconName { get; } = Option.ToString().ToLower();
}

public partial class ActorViewModel : ViewModelObjectBase {
	CancellationTokenSource? cts;

	[ObservableProperty] bool running;
	[ObservableProperty] ArgsViewModel editableArgs;
	[ObservableProperty] SettingsViewModel editableSettings;
	[ObservableProperty] BrowserOption browser;

	public IEnumerable<BrowserOption> BrowserOptions { get; } = [
		new (SystemBrowserType.Chrome),
		new (SystemBrowserType.Brave),
	];

	public IActor Actor { get; }
	public List<Selection> Selections { get; }
	public ReadOnlyObservableCollection<Tag> Tagz { get; }

	public ActorViewModel(IActor actor, IEnumerable<Selection>? initialSelections = null, IEnumerable<string>? initialSelectedTagNames = null, IEnumerable<int>? initialSelectedProfileIds = null) {
		Actor = actor;
		Selections = [.. actor.Scripts.Select(s =>{
				if (s is not Script script) return null;
				var selected = initialSelections?.FirstOrDefault(x => x.Script.File == script.File)?.Selected ?? false;
				return new Selection(script, selected);
			}).Where(s => s != null)
		];

		EditableArgs = new(actor.Options.Args);
		EditableSettings = new(actor.Options.Settings);
		Browser = BrowserOptions.First();

		_ = TagsRepo.Connect()
			.Filter(tag => tag.Items.Where(x => x.Key == TagItemType.Profile).Any())
			.Transform(item => new Tag(item, initialSelectedTagNames?.Contains(item.Name) ?? false))
			.Bind(out var tagz)
			.Subscribe();
		Tagz = tagz;

		if (initialSelectedProfileIds != null && MyProfilesViewModel.Instance.Profiles.Any()) {
			foreach (var profile in MyProfilesViewModel.Instance.Profiles) {
				if (profile.Dto?.id != null) {
					profile.IsSelected = initialSelectedProfileIds.Contains(profile.Dto.id);
				}
			}
		}

		AsyncCommandMap["Run"] = async () => {
			try {
				var profiles = GetProfilesForRun();
				if (profiles == null || profiles.Count == 0) {
					Debug.WriteLine("No profiles selected from tags or 'More Profiles' button. Prompting with main selector.");
					await OpenProfileSelectorDialogAsync();
					profiles = MyProfilesViewModel.Instance.Profiles.Where(p => p.IsSelected).ToList();

					if (profiles == null || profiles.Count == 0)
						throw new OperationCanceledException("No profiles selected for the run after all attempts.");
				}

				var selected = await EnsureScriptsSelectedAsync();

				Running = true;
				cts = new CancellationTokenSource();
				foreach (var profile in profiles) {
					cts.Token.ThrowIfCancellationRequested();

					var browser = await profile.OpenSystemBrowser(Browser.Option).WaitAsync(cts.Token);
					ArgumentNullException.ThrowIfNull(browser);

					foreach (var selection in selected) {
						cts.Token.ThrowIfCancellationRequested();

						var opts = new Opts(EditableArgs.ToDictionary(), EditableSettings.ToRecord());
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
			var currentOpts = new Opts(currentArgs, currentSettings);
			var stateToSave = new State(currentOpts, Selections, Tagz.Where(x => x.Selected), MyProfilesViewModel.Instance.Profiles.Where(x => x.IsSelected).Select(x => x.Dto.id));
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

		AsyncCommandMap["OpenProfileSelector"] = OpenProfileSelectorDialogAsync;
	}

	private async Task OpenProfileSelectorDialogAsync() {
		var profileIdsFromSelectedTags = Tagz
			.Where(x => x.Selected)
			.SelectMany(t => t.ProfileIds)
			.Distinct();
		var initiallySelectedForDialog = MyProfilesViewModel.Instance.Profiles
			.Where(p => p.IsSelected || profileIdsFromSelectedTags.Any(id => int.Parse(id) == p.Dto.id))
			.Distinct();
		using var profileSelectorVM = new ProfileSelectorViewModel(FoldersViewModel.Instance.Folders, MyProfilesViewModel.Instance.Profiles, initiallySelectedForDialog);

		var selectionMade = await profileSelectorVM.ShowDialogAsync();

		if (selectionMade) {
			Debug.WriteLine($"Profile selection dialog confirmed. {profileSelectorVM.SelectedProfiles.Count()} profiles now selected.");
		} else {
			Debug.WriteLine("Profile selection dialog cancelled or no changes made.");
		}
	}

	private List<ObsProfile>? GetProfilesForRun() {

		var selectedTagsInUI = Tagz.Where(t => t.Selected);
		var profileIdsFromSelectedTags = selectedTagsInUI
				.SelectMany(t => t.ProfileIds)
				.Distinct();

		List<ObsProfile> profilesToRun = [];

		if (profileIdsFromSelectedTags.Any()) {
			profilesToRun = MyProfilesViewModel.Instance.Profiles
					.Where(p => p.Dto != null && profileIdsFromSelectedTags.Contains(p.Dto.id.ToString()))
					.ToList();
			Debug.WriteLine($"Using {profilesToRun.Count} profiles from currently selected Tags: {string.Join(", ", selectedTagsInUI.Select(t => t.Dto.Name))}");
			profilesToRun.ForEach(p => p.IsSelected = true);
		}

		profilesToRun = [.. profilesToRun, .. MyProfilesViewModel.Instance.Profiles.Where(p => p.IsSelected)];
		if (profilesToRun.Count != 0) {
			Debug.WriteLine($"Using {profilesToRun.Count} profiles previously selected");
			return profilesToRun.Distinct().ToList();
		}

		Debug.WriteLine("No profiles selected from tags or previous dialog interaction.");
		return null;
	}

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
}
