using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Playwright.Models;

using Chameleon.client.Features.Automation.Playwright.Models;

namespace Chameleon.client.Features.Automation.Playwright.ViewModels;
public partial class ScriptViewModel(RunScriptOptions runOptions, List<ScriptParametersValues> saved) : ViewModelObjectBase(runOptions.Description?.Title) {
  public Action<string>? OnOpenEdit;

  [ObservableProperty]
  private string? description = runOptions.Description?.Description;

  [ObservableProperty]
  private string? filepath = runOptions.Description?.FilePath;

  public ObservableCollection<ScriptParametersValues> Parameters { get; } = [.. saved];

  public RunScriptOptions RunOptions => runOptions;
  public bool HasParameters => runOptions.Description?.Parameters.Count > 0;
  public string TableName => runOptions.BundledScript!.Name;

  [RelayCommand]
  public void OpenParamsPopup(string selectedScript) {
    OnOpenEdit?.Invoke(selectedScript);
  }
}