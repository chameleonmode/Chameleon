using Chameleon.AIR.Actors.Models;
using Chameleon.client.Features.Automation.Actors.Dialogs;
using Chameleon.client.Features.Automation.Actors.ViewModels;
using Chameleon.client.Features.Projects.Profiles;
using Chameleon.lib.AIR.Scripts.Models;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.client.MvvM;
using Chameleon.lib.Helpers;
using Chameleon.lib.Playwright.Services;
using Chameleon.lib.Util;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using DynamicData.Binding;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.Json.Serialization;
using Chameleon.lib.WebBrowser.Browsers;
using Chameleon.lib;
namespace Chameleon.client.Features.Automation.Actors;

public partial class Tag(TagDto dto, bool isSelected) : ObservableObject {
	[ObservableProperty] bool isSelected = isSelected;
	public TagDto Dto { get; } = dto;

	[JsonIgnore]
	public IEnumerable<string> ProfileIds => Dto.Items
	.Where(x => x.Key == TagItemType.Profile)
	.SelectMany(x => x.Value);

	[JsonIgnore] public string ToolTipText => $"{ProfileIds.Count()} Profiles";

	public void RaiseSelectedChanged() {
		OnPropertyChanged(nameof(IsSelected));
	}
}
public record Selection(Script Script, bool Selected = false);
public record State(Opts Options, IEnumerable<Selection> Selections, IEnumerable<Tag> SelectedTags, IEnumerable<int> SelectedProfileIds);

public partial class ActorViewModel : ViewModelObjectBase {
	readonly CompositeDisposable subscriptions = [];

	CancellationTokenSource? cts;

	[ObservableProperty] bool running;
	[ObservableProperty] AIR.Actors.Models.AI aiSettings;
	[ObservableProperty] ArgsViewModel editableArgs;
	[ObservableProperty] SettingsViewModel editableSettings;
	public IActor Actor { get; }
	public List<Selection> Selections { get; }

	public ReadOnlyObservableCollection<Tag> Tagz { get; }
	public ReadOnlyObservableCollection<ObsProfile> SelectedProfiles { get; }

	public ActorViewModel(
		IActor actor,
		IEnumerable<Selection>? selections = null,
		IEnumerable<string>? selectedTags = null,
		IEnumerable<int>? profileSelections = null
	) {
		subscriptions.Add(TagsRepo.Connect()
			.Filter(tag => tag.Items.Where(x => x.Key == TagItemType.Profile).Any())
			.Transform(item => new Tag(item, selectedTags?.Contains(item.Name) ?? false))
			.Bind(out var tagz)
			.Subscribe());
		Tagz = tagz;

		subscriptions.Add(ProfilesViewModel.Instance.ObsProfiles.ToObservableChangeSet()
			.AutoRefresh(profile => profile.IsSelected)
			.Filter(profile => profile.IsSelected)
			.Sort(SortExpressionComparer<ObsProfile>.Ascending(p => p.Title ?? ""))
			.DistinctUntilChanged()
			.Bind(out var selectedProfiles)
			.Subscribe());
		SelectedProfiles = selectedProfiles;

		subscriptions.Add(Tagz.ToObservableChangeSet()
			.AutoRefresh(tag => tag.IsSelected)
			.ToCollection()
			.Subscribe(next =>
				next.ForEach(t =>
					 ProfilesViewModel.Instance.ObsProfiles
					.Where(x => t.ProfileIds.Contains(x.Dto.ID))
					.ForEach(p => p.Active = p.IsSelected = t.IsSelected)
			)));

		profileSelections?.ForEach(id => {
			ProfilesViewModel.Instance.ObsProfiles
			.Where(p => p.Dto.id == id)
			.ForEach(p => p.IsSelected = true);
		});

		Actor = actor;
		AiSettings = actor.Options.AI;
		EditableArgs = new(actor.Options.Args);
		EditableSettings = new(actor.Options.Settings);
		Selections = [.. actor.Scripts.Select(s => {
			if (s is not Script script) return null;
				var selected = selections?.FirstOrDefault(x => x.Script.File == script.File)?.Selected ?? false;
				return new Selection(script, selected);
			}).Where(s => s != null)
		];

		AsyncCommandMap["Run"] = Runerer;
		AsyncCommandMap["Save"] = Save;
		AsyncCommandMap["OpenProfileSelector"] = async () => {
			using var profileSelectorVM = new ProfileSelectorViewModel(SelectedProfiles);
			_ = await profileSelectorVM.ShowDialogAsync();
		};

		CommandMap["Stop"] = Stop;
	}

	private async Task Save() {
		Actor.Options.Settings.Start.Feature.ThrowIfNullOrEmpty();
		var currentArgs = EditableArgs.ToDictionary([], EditableArgs.Search.Split(','));
		var currentSettings = EditableSettings.ToRecord();
		var currentOpts = new Opts(AiSettings, currentArgs, currentSettings);
		var stateToSave = new State(currentOpts, Selections, Tagz.Where(x => x.IsSelected), SelectedProfiles.Select(x => x.Dto.id));
		var filePath = Path.Combine(FilePaths.Roboto, $"{Actor.Options.Settings.Start.Feature}.json");
		var jsonContent = JS.Serialize(stateToSave, JS.EnumConverter);
		await File.WriteAllTextAsync(filePath, jsonContent, cts?.Token ?? CancellationToken.None);
	}

	public async Task Runerer() {
		cts = new CancellationTokenSource();
		Running = true;
		try {
			var profiles = SelectedProfiles.Where(p => p.Active);
			if (!profiles.Any()) throw new Exception("No profiles selected to run.");

			var selected = Selections.OrderBy(s => new Random().Next()).Where(s => s.Selected);
			if (!selected.Any()) throw new Exception("No scripts selected to run.");

			var things = EditableArgs.Search.Is() && EditableSettings.Start.Url.Is();
			if (things) throw new Exception("Search and URL's cannot be empty together.");
			var executionIndex = -1;
			var terms = EditableArgs.Search.Contains(',') ? EditableArgs.Search.Split(",").Select(x => x.Trim()) : [EditableArgs.Search.Trim()];
			var urls = EditableSettings.Start.Url?.Split('\n').Where(x => x.IsNot()).Select(x => x.Trim()) ?? [];

			async Task<IBrowserInstance?> ExecuteScriptAsync(Selection selection, ObsProfile profile) {
				Toaster.Info($"Starting '{selection.Script.Title}");
				if (executionIndex++ >= terms.Count() && executionIndex >= terms.Count()) executionIndex = 0;

				var termer = !EditableSettings.AsQue ? terms
				: executionIndex >= terms.Count() ? []
				: [terms.ElementAt(executionIndex)];

				EditableSettings.Start.Urls = !termer.Any() && !EditableSettings.AsQue ? urls
				: executionIndex >= urls.Count() ? []
				: [urls.ElementAt(executionIndex)];

				var opts = new Opts(AiSettings, EditableArgs.ToDictionary(selected, termer), EditableSettings.ToRecord(selection.Script.Title == "Surf" ? new(0, 0) : null));
				Debug.WriteLine($"Running: \n\t '{profile.Title}', '{selection.Script.Title}', '{opts.Settings.Start.Feature}', {JS.Serialize(opts)}");

				var browser = await profile.OpenSystemBrowser(ActorsViewModel.Instance.Browser.Option, false).WaitAsync(cts!.Token);
				await Run.Script(new() { Port = browser!.Settings.Port, Script = selection.Script, Opts = opts }, cts!.Token);
				await Task.Delay(TimeSpan.FromSeconds(EditableSettings.Delay), cts.Token);
				Toaster.Info($"Finished Script '{selection.Script.Title}'");
				return browser;
			}
			async Task BrowserShutdown(IBrowserInstance? browser) {
				if (!EditableSettings.CloseOldBrowserProfileAfterRun || browser?.Brocess == null || browser.Brocess.HasExited) return;
				try {
					// First, try to close the main window gracefully
					if (browser.Brocess.CloseMainWindow()) {
						// Wait for the process to exit gracefully
						if (await Task.Run(() => browser.Brocess.WaitForExit(5000))) {
							Debug.WriteLine("Process closed gracefully.");
						}
					}

					// If graceful close failed or timed out, force kill
					Debug.WriteLine("Graceful close failed. Force killing process...");
					browser.Brocess.Kill();
					browser.Close();

					// Wait for the kill to complete
					_ = await Task.Run(() => browser.Brocess?.WaitForExit(5000));
					Debug.WriteLine("Process forcefully terminated.");
				} catch (Exception e) {
					Debug.WriteLine($"Error closing browser: {e.Message}");
				} finally {
					Debug.WriteLine("Browser instance disposed.");
				}
			}

			if (EditableSettings.EachProfile) foreach (var selection in selected) {
					foreach (var profile in profiles) {
						var browser = await ExecuteScriptAsync(selection, profile);
						await BrowserShutdown(browser);
					}
				}
			else foreach (var profile in profiles) {
					IBrowserInstance? browser = null;
					foreach (var selection in selected) {
						browser = await ExecuteScriptAsync(selection, profile);
					}
					await BrowserShutdown(browser);
				}
		} finally { Stop(); }
	}

	private void Stop() {
		cts?.Cancel();
		cts?.Dispose();
		cts = null;
		Running = false;
	}
}