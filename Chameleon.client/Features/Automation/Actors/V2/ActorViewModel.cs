using Chameleon.AIR.Actors.Models;
using Chameleon.AIR.Scripts.Models;
using Chameleon.app.Avalonia.Controls;
using Chameleon.app.Avalonia.ViewModels.Controllers;
using Chameleon.client.Features.Automation.Actors.ViewModels;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace Chameleon.client.Features.Automation.Actors.V2;
public record Selection(IScript Script, bool Selected = false);

public partial class ActorViewModel : ViewModelObjectBase {
	CancellationTokenSource? cts;

	[ObservableProperty]
	bool running;

	public IActor Actor { get; }

	[ObservableProperty]
	ArgsViewModel _editableArgs = new();

	[ObservableProperty]
	SettingsViewModel _editableSettings = new();

	public List<Selection> Selections { get; } = [];

	public ActorViewModel(IActor actor) {
		Actor = actor;

		foreach (var script in actor.Scripts) {
			Selections.Add(new Selection(script));
		}

		_editableArgs = new(actor.Options.Args);
		_editableSettings = new(actor.Options.Settings);

		AsyncCommandMap["Run"] = async () => {
			Running = true;

			var selectedScripts = Selections.Where(s => s.Selected).Select(s => s.Script).ToList();
			if (selectedScripts.Count == 0) {
				Running = false;
				throw new Exception("No scripts selected.");
			}

			var profileOptionsVM = new InviteUserOrAddProfilesViewModel(true) { ShowUserInfo = false };
			if (
				await Mbox.ShowTaskDialog<InviteUserOrAddProfilesViewModel, InviteUserOrAddProfilesUserControl>(
					initialize: () => profileOptionsVM,
					header: "Add Profiles & Folders",
					subHeader: "Add profiles and folders to run these automationairs.",
					symbas: Enums.Symbas.AddFriend,
					btns: Enums.MBoxButtons.OkCancel) == Enums.TaskDialogResult.OK
			) {
				cts = new CancellationTokenSource();
				try {
					foreach (var profile in profileOptionsVM.SelectedProfiles) {
						cts.Token.ThrowIfCancellationRequested();

						// var browser = await profile.OpenSystemBrowser(Enums.SystemBrowserType.Chrome).WaitAsync(cts.Token);
						// ArgumentNullException.ThrowIfNull(browser);

						foreach (var scriptToRun in selectedScripts) {
							cts.Token.ThrowIfCancellationRequested();

							var argsToUse = EditableArgs.ToDictionary();
							var settingsToUse = EditableSettings.ToRecord();

							var optionsToUse = new Opts(argsToUse, settingsToUse);

							await Task.Delay(1000, cts.Token);
							Debug.WriteLine($"Simulating run: Profile '{profile.Title}', Script '{scriptToRun.Title}' with Feature '{optionsToUse.Settings.Start.Feature}'");

						}
					}
				} catch (OperationCanceledException) {
					Debug.WriteLine("Run cancelled.");
				} catch (Exception ex) {
					Debug.WriteLine($"Error during run: {ex.Message}");
				} finally {
					StopExecution();
				}
			} else {
				Running = false;
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
