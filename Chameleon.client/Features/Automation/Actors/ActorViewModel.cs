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
				var selected = Selections.Where(s => s.Selected);
				if (!selected.Any()) throw new Exception("No scripts selected.");

				if (await new InviteUserOrAddProfilesViewModel().ShowDialog() is { } result) {
					Running = true;
					cts = new CancellationTokenSource();
					foreach (var profile in result.SelectedProfiles) {
						cts.Token.ThrowIfCancellationRequested();

						var browser = await profile.OpenSystemBrowser(Enums.SystemBrowserType.Chrome).WaitAsync(cts.Token);
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
				}
			} finally {
				StopExecution();
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
