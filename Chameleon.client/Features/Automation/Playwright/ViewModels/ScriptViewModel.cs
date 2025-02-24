using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Playwright.Models;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.client.Features.Automation.Playwright.ViewModels;
public partial class ScriptViewModel(RunScriptOptions runOptions) : ViewModelObjectBase(runOptions.Description?.Title) {
  public Action<string>? OnOpenEdit;

  [ObservableProperty]
  private string? description = runOptions.Description?.Description;

  [ObservableProperty]
  private string? filepath = runOptions.Description?.FilePath;

  //[ObservableProperty]
  public Dictionary<string, string>? Parameters => runOptions.Description?.Parameters;

  public bool HasParameters => runOptions.Description?.Parameters.Count > 0;

  public RunScriptOptions RunOptions => runOptions;

  [RelayCommand]
  public void OpenParamsPopup(string selectedScript) {
    OnOpenEdit?.Invoke(selectedScript);
  }
}