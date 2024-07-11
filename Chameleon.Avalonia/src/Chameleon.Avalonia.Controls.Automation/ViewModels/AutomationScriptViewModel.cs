using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.App.Automation.Services;
using Chameleon.Interfaces.App.Automation.ViewModels;
using Chameleon.Interfaces.App.Automation.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Avalonia.Controls.Automation.ViewModels;
public partial class AutomationScriptViewModel(IAutomationScriptDescription automationScriptDescription)
    : SubPageViewModelBase
    , IAutomationScriptViewModel
{
    [ObservableProperty]
    private int _id = automationScriptDescription.Id;

    [ObservableProperty]
    private string _description = automationScriptDescription.Description;

    [ObservableProperty]
    private string _filepath = automationScriptDescription.FilePath;

    [ObservableProperty]
    private IList<IAutomationScriptParameterViewModel> _parameters = 
        automationScriptDescription.Parameters
            .Select(param => new AutomationScriptParameterViewModel(param))
            .ToList<IAutomationScriptParameterViewModel>();

    [ObservableProperty]
    private IAutomationScriptDescription _scriptDescription = automationScriptDescription;

    [ObservableProperty]
    private new string title = automationScriptDescription.Title;

    public bool IsHasParameter => Parameters.Count != 0;

    [RelayCommand]
    public async Task OpenParamsPopup(int selectedIdScript)
    {
        var result = await ContentDialogService
            .ShowAsync<IAddScriptParametersPopupView, IAddScriptParametersPopupViewModel>(viewModel =>
           {
               viewModel.Title = "Set Script Parameters";
               viewModel.ScriptDescription = ScriptDescription;
           });
    }
}
