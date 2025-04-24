using Chameleon.AIR.Actors.Models;
using Chameleon.AIR.Scripts.Models;
using Chameleon.app.Avalonia.Controls;
using Chameleon.app.Avalonia.ViewModels.Controllers;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Playwright.Utils;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.client.Features.Automation.Actors;

public record Opts(string Key, string Value);
public record Options(string Key, List<Opts> Value);
public record Selection(IScript Script, bool Selected = false);

// TODO: This is a temporary view model for the actor. It should be replaced with a more
// comprehensive view model that handles the actor's properties and methods.
public partial class ActorViewModel : ViewModelObjectBase {
  [ObservableProperty]
  bool running;

  public IActor Actor { get; }
  public List<Options> Options { get; } = [];
  public List<Selection> Selections { get; } = [];

  CancellationTokenSource? cts;

  public ActorViewModel(IActor actor) {
    Actor = actor;

    foreach (var script in actor.Scripts) {
      Selections.Add(new Selection(script));
    }

    AddOptions(actor.Options, "");

    AsyncCommandMap["Run"] = async () => {
      var selected = Selections.Where(s => s.Selected);
      if (!selected.Any()) throw new Exception("No scripts selected.");
      
      var options = new InviteUserOrAddProfilesViewModel(true) {
        ShowUserInfo = false,
      };
      if (
        await Mbox.ShowTaskDialog<InviteUserOrAddProfilesViewModel, InviteUserOrAddProfilesUserControl>(
          initialize: () => options,
          header: "Add Profiles & Folders",
          subHeader: "Add profiles and folders to run these automationairs.",
          symbas: Enums.Symbas.AddFriend,
          btns: Enums.MBoxButtons.OkCancel) == Enums.TaskDialogResult.OK
      ) {
        cts = new CancellationTokenSource();

        foreach (var profile in options.SelectedProfiles) {
          cts.Token.ThrowIfCancellationRequested();

          var browser = await profile.OpenSystemBrowser(Enums.SystemBrowserType.Chrome).WaitAsync(cts.Token);
          ArgumentNullException.ThrowIfNull(browser);

          foreach (var selection in selected) {
            cts.Token.ThrowIfCancellationRequested();

            await PlaywriteRunner.RunScript(new() {
              Port = browser.Settings.Port,
              BundledScript = selection.Script,
              Opts = Options
            }, cts.Token);
          }
        }
      }
    };
    AsyncCommandMap["Stop"] = () => {
      if (cts != null) {
        cts.Cancel();
        cts.Dispose();
        cts = null;
      }
      return Task.CompletedTask;
    };
  }

  private void AddOptions(object obj, string prefix) {
    if (obj == null) return;

    var type = obj.GetType();
    foreach (var prop in type.GetProperties()) {
      var key = $"{prop.Name}";
      var value = prop.GetValue(obj);

      // Continue recursion for complex objects
      if (value != null && !IsSimpleType(value.GetType())) {
        AddOptions(value, key);
      } else {
        var opts = Options.FirstOrDefault(o => o.Key == prefix);
        if (opts == null) {
          opts = new Options(prefix, []);
          Options.Add(opts);
        }

        opts.Value.Add(new Opts(key, value?.ToString() ?? "null"));
      }
    }
  }

  private bool IsSimpleType(Type type) {
    return type.IsPrimitive ||
           type == typeof(string) ||
           type == typeof(decimal) ||
           type.IsEnum ||
           Nullable.GetUnderlyingType(type) != null ||
           type == typeof(DateTime) ||
           type == typeof(DateTimeOffset) ||
           type == typeof(TimeSpan) ||
           type == typeof(Guid);
  }
}

// TODO: ? Initialize the Opts collection like this maybe?
// public record Args(Opts[] Options, Opts[] Start, Opts[] Tiemouts, Opts[] Waits, Opts[] Interactions, Opts[] Iterations);
// public record Settings();
// public Args Args { get; }
// Args = new Args(
//   Options: AddPropertiesRecursively(actor.Options.Args, []),
//   Start: AddPropertiesRecursively(actor.Options.Settings.Start, []),
//   Tiemouts: AddPropertiesRecursively(actor.Options.Settings.Start, []),
//   Waits: AddPropertiesRecursively(actor.Options.Settings.Start, []),
//   Interactions: AddPropertiesRecursively(actor.Options.Settings.Start, []),
//   Iterations: AddPropertiesRecursively(actor.Options.Settings.Start, [])
// );
// private Opts[] AddPropertiesRecursively(object obj, List<Opts> opts) {
//   if (obj == null) return [.. opts];

//   Type type = obj.GetType();
//   foreach (PropertyInfo prop in type.GetProperties()) {
//     string key = $"{prop.Name}";
//     object? value = prop.GetValue(obj);
//     string valueStr = value?.ToString() ?? "null";

//     // Continue recursion for complex objects
//     if (value != null && !IsSimpleType(value.GetType())) {
//       return AddPropertiesRecursively(value, opts);
//     } else {
//       opts.Add(new Opts(key, valueStr));
//     }
//   }

//   return [.. opts];
// }