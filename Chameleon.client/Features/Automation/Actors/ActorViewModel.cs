using Chameleon.AIR.Actors.Models;
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
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using System.Collections.ObjectModel;
using System.Diagnostics;
using static Chameleon.lib.Common.Constants.Enums;
namespace Chameleon.client.Features.Automation.Actors;
// multiple storage state / results
// more ai stuffs
public record Tag(TagDto Dto, bool Selected = false) {
	public TagItemDto[] Items { get; } =
		[.. Dto.Items.Where(x => x.Key == TagItemType.Profile).Select(x => new TagItemDto(x.Key, x.Value))];
}
public record Selection(Script Script, bool Selected = false);
public record State(Opts Options, IEnumerable<Selection> Selections, IEnumerable<Tag> Tags);
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

	public ActorViewModel(IActor actor, IEnumerable<Selection>? selections = null, IEnumerable<Tag>? selectedTags = null) {
		Actor = actor;
		Selections = [.. actor.Scripts.Select(s =>{
				if (s is not Script script) return null;
				var selected = selections?.FirstOrDefault(x => x.Script.File == script.File)?.Selected ?? false;
				return new Selection(script, selected);
			}).Where(s => s != null)
		];

		EditableArgs = new(actor.Options.Args);
		EditableSettings = new(actor.Options.Settings);
		Browser = BrowserOptions.First();

		_ = TagsRepo.Connect()
			.Filter(tag => tag.Items.Where(x => x.Key == TagItemType.Profile).Any())
			.Transform(item => new Tag(item, selectedTags?.Any(x => x.Dto.Name == item.Name) ?? false))
			.Bind(out var tagz)
			.Subscribe();
		Tagz = tagz;

		AsyncCommandMap["Run"] = async () => {
			try {

				var profiles = await EnsureProfilesSelectedAsync();
				if (profiles == null || profiles.Count == 0) 
					throw new OperationCanceledException("No profiles selected for the run");

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
			var stateToSave = new State(currentOpts, Selections, Tagz.Where(x => x.Selected));
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

	async Task<List<ObsProfile>> EnsureProfilesSelectedAsync() {

		var allProfiles = MyProfilesViewModel.Instance.Profiles;
		var allFolders = FoldersViewModel.Instance.Folders;
		var profilesFromTags = Tagz
						.Where(t => t.Selected)
						.SelectMany(t => t.Items)
						.SelectMany(i => i.Ids)
						.Distinct()
						.Select(id => allProfiles.FirstOrDefault(p => p.Dto?.id.ToString() == id))
						.Where(p => p != null)
						.ToList();

		if (profilesFromTags.Count != 0) {
			return profilesFromTags!;
		}

		if (allFolders == null || allProfiles == null) {
			Debug.WriteLine("Error: Profile/Folder collections not available for dialog");
			throw new Exception("Could not load profiles/folders for selection");
		}

		using var profileSelectorVM = new ProfileSelectorViewModel(allFolders, allProfiles);
		var selectionMade = await profileSelectorVM.ShowDialogAsync();

		if (selectionMade && profileSelectorVM.SelectedProfiles.Any()) {
			Debug.WriteLine($"Using {profileSelectorVM.SelectedProfiles.Count()} profiles selected via Dialog");
			return profileSelectorVM.SelectedProfiles.ToList();
		}

		Debug.WriteLine("No profiles selected from dialog or dialog cancelled.");
		return [];
	}
}
