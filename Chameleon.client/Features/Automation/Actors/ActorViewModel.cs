using Chameleon.AIR.Actors.Models;
using Chameleon.AIR.Scripts.Models;
using Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.MyProfiles;
using Chameleon.app.Avalonia.ViewModels.Controllers;
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
// tooltips
// multi kw by ',' delimiter or ai generate or both...
// multiple storage state / results
// if start url start script at url and skip search or find post if the url is checked
// more ai stuffs
public record Tag(TagDto Dto, bool Selected = false) {
	public TagItemDto[] Items { get; } =
		[.. Dto.Items.Where(x => x.Key == TagItemType.Profile).Select(x => new TagItemDto(x.Key, x.Value))];
}

public record Selection(IScript Script, bool Selected = false);
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

	public ActorViewModel(IActor actor) {
		Actor = actor;
		Selections = [.. Actor.Scripts.Select(script => new Selection(script))];

		EditableArgs = new(actor.Options.Args);
		EditableSettings = new(actor.Options.Settings);
		Browser = BrowserOptions.First();

		_ = TagsRepo.Connect()
			.Filter(tag => tag.Items.Where(x => x.Key == TagItemType.Profile).Any())
			.Transform(item => new Tag(item))
			.Bind(out var tagz)
			.Subscribe();
		Tagz = tagz;

		AsyncCommandMap["Run"] = async () => {
			try {
				var selected = Selections.Where(s => s.Selected);
				if (!selected.Any()) throw new Exception("No scripts selected.");

				MyProfilesViewModel.Instance.PaginatorViewModel.UpdatePageCount(UserProfilesRepo.Instance.ObservableCache.Count);
				var profiles = Tagz.Where(t => t.Selected)
				 	.SelectMany(t => t.Items)
					.SelectMany(i => i.Ids)
					.Select(
						id => MyProfilesViewModel.Instance.Profiles.First(p => p.Dto.id.ToString() == id)
					);
				if (!profiles.Any()) profiles = (await new InviteUserOrAddProfilesViewModel().ShowDialog())?.SelectedProfiles;
				if (profiles == null) throw new Exception("No profiles selected.");

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
				if (Running) StopExecution();
				// TODO: save the actor state
				// await File.WriteAllTextAsync(
				// 	Path.Combine(FilePaths.Roboto, Actor.Options.Settings.Start.Feature + ".json"),
				// 	JS.Serialize(Actor)
				// );
			}
		};

		CommandMap["Stop"] = () => {
			StopExecution();
		};
	}

	private void StopExecution() {
		if (cts != null) {
			cts.Cancel();
			cts.Dispose();
			cts = null;
		}
		Running = false;
	}
}
