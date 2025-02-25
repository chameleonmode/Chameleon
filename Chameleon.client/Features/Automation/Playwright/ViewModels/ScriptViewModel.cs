using System.Collections.ObjectModel;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Playwright.Models;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.client.Features.Automation.Playwright.ViewModels;
public record ScriptParametersValues(string? Key, string? Value);
public partial class ScriptViewModel(RunScriptOptions runOptions) : ViewModelObjectBase(runOptions.Description?.Title) {
  public Action<string>? OnOpenEdit;

  [ObservableProperty]
  private string? description = runOptions.Description?.Description;

  [ObservableProperty]
  private string? filepath = runOptions.Description?.FilePath;

  public ObservableCollection<ScriptParametersValues> Parameters { get; } = [..runOptions.Description!.Parameters.Select(p => new ScriptParametersValues( Key: p.Key, Value: p.Value ))];

  public RunScriptOptions RunOptions => runOptions;
  public bool HasParameters => runOptions.Description?.Parameters.Count > 0;

  [RelayCommand]
  public void OpenParamsPopup(string selectedScript) {
    OnOpenEdit?.Invoke(selectedScript);
  }
}