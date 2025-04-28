using Chameleon.AIR.Actors.Models;
using Chameleon.AIR.Scripts.Models;
using Chameleon.app.Avalonia.Controls;
using Chameleon.app.Avalonia.ViewModels.Controllers;
using Chameleon.client.Features.Automation.Actors.ViewModels;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Const;
using Chameleon.lib.Playwright.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace Chameleon.client.Features.Automation.Actors;

public record Selection(IScript Script, bool Selected = false);

public partial class ActorViewModel : ViewModelObjectBase {
	CancellationTokenSource? cts;

	[ObservableProperty] bool running;
	[ObservableProperty] ArgsViewModel editableArgs;
	[ObservableProperty] SettingsViewModel editableSettings;

	public IActor Actor { get; }
	public List<Selection> Selections { get; }

	public ActorViewModel(IActor actor) {
		Actor = actor;
		Selections = [.. Actor.Scripts.Select(script => new Selection(script))];

		EditableArgs = new(actor.Options.Args);
		EditableSettings = new(actor.Options.Settings);

		AsyncCommandMap["Run"] = async () => {
			try {
				var selectedScripts = Selections.Where(s => s.Selected);
				if (!selectedScripts.Any()) throw new Exception("No scripts selected.");

				var profileOptions = new InviteUserOrAddProfilesViewModel(true) { ShowUserInfo = false };
				if (
					await Mbox.ShowTaskDialog<InviteUserOrAddProfilesViewModel, InviteUserOrAddProfilesUserControl>(
						initialize: () => profileOptions,
						header: "Add Profiles & Folders",
						subHeader: "Add profiles and folders to run these automationairs.",
						symbas: Enums.Symbas.AddFriend,
						btns: Enums.MBoxButtons.OkCancel) == Enums.TaskDialogResult.OK
				) {
					cts = new CancellationTokenSource();
					foreach (var profile in profileOptions.SelectedProfiles) {
						cts.Token.ThrowIfCancellationRequested();

						// var browser = await profile.OpenSystemBrowser(Enums.SystemBrowserType.Chrome).WaitAsync(cts.Token);
						// ArgumentNullException.ThrowIfNull(browser);

						foreach (var scriptToRun in selectedScripts) {
							cts.Token.ThrowIfCancellationRequested();
							Running = true;

							var args = EditableArgs.ToDictionary();
							var settings = EditableSettings.ToRecord();
							var opts = new Opts(args, settings);
							var json = JS.Serialize(opts);
							Debug.WriteLine($@"Running script with:
							  Profile '{profile.Title}', Script '{scriptToRun.Script.Title}' with Feature '{opts.Settings.Start.Feature}'");
							Debug.WriteLine(json);
							await Task.Delay(1000, cts.Token);

							// await PlaywriteRunner.RunScript(new() {
							// 	Port = browser.Settings.Port,
							// 	Script = selection.Script,
							// 	Opts = opts
							// }, cts.Token);
						}
					}
				}
			} finally {
				StopExecution();
			}
		};

		AsyncCommandMap["Stop"] = () => {
			StopExecution();
			return Task.CompletedTask;
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
