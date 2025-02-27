using System.Collections.ObjectModel;
using Chameleon.client.Features.Automation.Playwright.Models;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Playwright.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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

  [RelayCommand]
  public void OpenParamsPopup(string selectedScript) {
    OnOpenEdit?.Invoke(selectedScript);
  }
}