using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Avalonia.Collections;

using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Playwright.Models;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.app.Avalonia.ViewModels.Playwright;
public partial class AutomationScriptViewModel(PlaywriteRunScriptOptions runOptions) : PageViewModelBase(runOptions.Description?.Title)
{
	public Action<string> OnOpenEdit;

  [ObservableProperty]
  private string? description = runOptions.Description?.Description;

  [ObservableProperty]
  private string? filepath = runOptions.Description?.FilePath;

	[ObservableProperty]
	public AvaloniaList<PlaywrightDescriptionParam> parameters = [];

  public bool IsHasParameter => runOptions.Description?.Parameters.Count != 0;

	public PlaywriteRunScriptOptions RunOptions => runOptions;

	[RelayCommand]
  public void OpenParamsPopup(string selectedScript)
  {
		OnOpenEdit?.Invoke(selectedScript);
	}
}
