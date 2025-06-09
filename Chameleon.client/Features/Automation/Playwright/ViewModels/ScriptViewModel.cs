using System.Collections.ObjectModel;
using Chameleon.client.Features.Automation.Playwright.Models;
using Chameleon.client.MvvM;
using Chameleon.lib.Playwright;
using Chameleon.lib.Playwright.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.client.Features.Automation.Playwright.ViewModels;
public partial class ScriptViewModel(Arguments args, List<ScriptParametersValues> saved) : ViewModelObjectBase(args.Description?.Title) {
  public Action<string>? OnOpenEdit;

  [ObservableProperty]
  private string? description = args.Description?.Description;

  [ObservableProperty]
  private string? filepath = args.Description?.FilePath;

  public ObservableCollection<ScriptParametersValues> Parameters { get; } = [.. saved];

  public Arguments RunOptions => args;
  public bool HasParameters => args.Description?.Parameters.Count > 0;

  [RelayCommand]
  public void OpenParamsPopup(string selectedScript) {
    OnOpenEdit?.Invoke(selectedScript);
  }
}