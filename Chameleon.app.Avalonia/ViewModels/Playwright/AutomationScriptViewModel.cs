using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Avalonia.Collections;

using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Core.Automation.Interfaces;
using Chameleon.lib.Playwright.Interfaces;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.app.Avalonia.ViewModels.Playwright;
public partial class AutomationScriptViewModel(IPlaywriteRunScriptOptions automationScriptDescription) : PageViewModelBase(automationScriptDescription.Script?.Title)
{
	public Action<string> OnOpenEdit;

  [ObservableProperty]
  private IAutomationScriptDescription? _scriptDescription = automationScriptDescription.Script;

  [ObservableProperty]
  private int? _id = automationScriptDescription.Script?.Id;

  [ObservableProperty]
  private string? _description = automationScriptDescription.Script?.Description;

  [ObservableProperty]
  private string? _filepath = automationScriptDescription.Script?.FilePath;

  [ObservableProperty]
  private IList<IAutomationParameterValue>? _parameters = new AvaloniaList<IAutomationParameterValue>(automationScriptDescription.Script?.Parameters);

  public bool IsHasParameter => automationScriptDescription.Script?.Parameters?.Count != 0;

  [RelayCommand]
  public void OpenParamsPopup(string selectedScript)
  {
		OnOpenEdit?.Invoke(selectedScript);
		//var result = await ContentDialogService
		//		.ShowAsync<IAddScriptParametersPopupView, IAddScriptParametersPopupViewModel>(viewModel =>
		//		{
		//			viewModel.Title = "Set Script Parameters";
		//			viewModel.ScriptDescription = ScriptDescription;
		//		});
	}
}
