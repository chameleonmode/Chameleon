using System.Linq;
using Chameleon.AIR.Actors.Models;
using Chameleon.AIR.Scripts.Models;
using Chameleon.app.Avalonia.Controls;
using Chameleon.app.Avalonia.ViewModels.Controllers;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Const;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.client.Features.Automation.Actors;

public record KeyValues(string Key, string Value);
public record Options(string Prefix, string Key, List<KeyValues> Value);
public record Selection(IScript Script, bool Selected = false);

// TODO: This is a temporary view model for the actor. It should be replaced with a more
// comprehensive view model that handles the actor's properties and methods.
public partial class ActorViewModel : ViewModelObjectBase {
  CancellationTokenSource? cts;

  [ObservableProperty]
  bool running;

  public IActor Actor { get; }
  public List<Options> Options { get; } = [];
  public List<Selection> Selections { get; } = [];

  public ActorViewModel(IActor actor) {
    Actor = actor;

    foreach (var script in actor.Scripts) {
      Selections.Add(new Selection(script));
    }

    ConvertFromActorOptions(actor.Options);

    // AddOptions(actor.Options, "");

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

            var args = new Dictionary<string, object>();
            foreach (var opt in Options.Where(o => o.Prefix == "Args")) {
              foreach (var kv in opt.Value) {
                args.Add(kv.Key, kv.Value);
              }
            }
            var o = new Opts(
              Args: Options.Where(o => o.Prefix == "Args").FirstOrDefault()?.Value.ToDictionary(k => k.Key, k => (object)k.Value) ?? Actor.Options.Args, //(k => new KeyValuePair<string, object>(k.Key, k.Value)).FirstOrDefault(),
              Settings: new(
                Start: Options.Where(o => o.Prefix == "Start").Select(o => new Start(
                  Feature: o.Value.FirstOrDefault(k => k.Key == "Feature")?.Value ?? Actor.Options.Settings.Start.Feature,
                  Url: o.Value.FirstOrDefault(k => k.Key == "Url")?.Value,
                  New: bool.TryParse(o.Value.FirstOrDefault(k => k.Key == "New")?.Value, out var newBool) ? newBool : null
                )).FirstOrDefault() ?? Actor.Options.Settings.Start,
                Timeouts: Options.Where(o => o.Prefix == "Timeouts").Select(o => new Timeouts(
                  Default: int.TryParse(o.Value.FirstOrDefault(k => k.Key == "Default")?.Value, out var defaultInt) ? defaultInt : Actor.Options.Settings.Timeouts.Default,
                  Wait: int.TryParse(o.Value.FirstOrDefault(k => k.Key == "Wait")?.Value, out var waitInt) ? waitInt : Actor.Options.Settings.Timeouts.Wait,
                  Navigate: int.TryParse(o.Value.FirstOrDefault(k => k.Key == "Navigate")?.Value, out var navigateInt) ? navigateInt : Actor.Options.Settings.Timeouts.Navigate,
                  Naps: Options.Where(k => k.Key == "Naps").Select(o => new Rando(
                    Min: int.TryParse(o.Value.FirstOrDefault(k => k.Key == "Min")?.Value, out var min) ? min : Actor.Options.Settings.Timeouts.Naps.Min,
                    Max: int.TryParse(o.Value.FirstOrDefault(k => k.Key == "Max")?.Value, out var max) ? max : Actor.Options.Settings.Timeouts.Naps.Max,
                    Multiplier: int.TryParse(o.Value.FirstOrDefault(k => k.Key == "Multiplier")?.Value, out var multiplier)  ? multiplier : Actor.Options.Settings.Timeouts.Naps.Multiplier
                  )).FirstOrDefault() ?? Actor.Options.Settings.Timeouts.Naps
                )).FirstOrDefault() ?? Actor.Options.Settings.Timeouts,
                Rando: Options.Where(o => o.Prefix == "Rando").Select(o => new Rando(
                  Min: int.TryParse(o.Value.FirstOrDefault(k => k.Key == "Min")?.Value, out var minRandoInt) ? minRandoInt : Actor.Options.Settings.Rando.Min,
                  Max: int.TryParse(o.Value.FirstOrDefault(k => k.Key == "Max")?.Value, out var maxRandoInt) ? maxRandoInt : Actor.Options.Settings.Rando.Max,
                  Multiplier: int.TryParse(o.Value.FirstOrDefault(k => k.Key == "Multiplier")?.Value, out var multiplierRandoInt) ? multiplierRandoInt : Actor.Options.Settings.Rando.Multiplier
                )).FirstOrDefault() ?? Actor.Options.Settings.Rando,
                Iterations: Options.Where(o => o.Prefix == "Iterations").Select(o => new Rando(
                    Min: int.TryParse(o.Value.FirstOrDefault(k => k.Key == "Min")?.Value, out var min) ? min : Actor.Options.Settings.Iterations.Min,
                    Max: int.TryParse(o.Value.FirstOrDefault(k => k.Key == "Max")?.Value, out var max) ? max : Actor.Options.Settings.Iterations.Max,
                    Multiplier: int.TryParse(o.Value.FirstOrDefault(k => k.Key == "Multiplier")?.Value, out var multiplier)  ? multiplier : Actor.Options.Settings.Iterations.Multiplier
                )).FirstOrDefault() ?? Actor.Options.Settings.Iterations
              )
            );
            var json = JS.Serialize(Actor.Options);

            // await PlaywriteRunner.RunScript(new() {
            //   Port = browser.Settings.Port,
            //   BundledScript = selection.Script,
            //   Opts = ActorOptions
            // }, cts.Token);
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

  // Add a property that performs the conversion
  // Add direct access to the Settings
  private void ToOptions(object obj, string prefix) {
    if (obj == null) return;

    var type = obj.GetType();
    foreach (var prop in type.GetProperties()) {
      var key = $"{prefix}.{prop.Name}";
      var value = prop.GetValue(obj);

      // Continue recursion for complex objects
      if (value != null && !IsSimpleType(value.GetType())) {
        ToOptions(value, key);
      } else {
        var opts = Options.FirstOrDefault(o => o.Key == prefix);
        if (opts == null) {
          opts = new Options(key, prop.Name, []);
          Options.Add(opts);
        }

        opts.Value.Add(new KeyValues(key, value?.ToString() ?? "null"));
      }
    }
  }

  // From List<Options> to Opts Settings using recursion for nested types

  
  // Convert from Opts to List<Options>
  private void ConvertFromActorOptions(Opts actorOptions) {
    // Clear existing options
    Options.Clear();
    var opts = new Options("Args", "Options", [.. actorOptions.Args.Select(kvp => new KeyValues(kvp.Key, kvp.Value.ToString()))]);
    Options.Add(opts);

    // Also add settings
    ToOptions(actorOptions.Settings, "Settings");
  }

  private object? ParseValue(string? value) {
    // Try to parse the value as a simple type
    if (int.TryParse(value, out var intValue)) return intValue;
    if (bool.TryParse(value, out var boolValue)) return boolValue;
    if (double.TryParse(value, out var doubleValue)) return doubleValue;
    if (DateTime.TryParse(value, out var dateTimeValue)) return dateTimeValue;

    // If parsing fails, return the original string
    return value;
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