using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.App.Automation.Services;
using Chameleon.Interfaces.App.Automation.ViewModels;
using Chameleon.Interfaces.App.Automation.Views;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Avalonia.Controls.Automation.ViewModels;
public partial class AutomationScriptViewModel
    : SubPageViewModelBase
    , IAutomationScriptViewModel
{
    private readonly IAutomationService _automationService;

    public AutomationScriptViewModel(
        IAutomationScriptDescription automationScriptDescription,
        IAutomationService automationService
        )
    {
        _automationService = automationService;

        Id = automationScriptDescription.Id;
        Title = automationScriptDescription.Title;
        Description = automationScriptDescription.Description;
        Parameters = automationScriptDescription.Parameters
            .Select(param => new AutomationScriptParameterViewModel(param))
            .ToList<IAutomationScriptParameterViewModel>();

        ScriptDescription = automationScriptDescription;
    }

    private int _id;
    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    private string _description;
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    private IList<IAutomationScriptParameterViewModel> _parameters;
    public IList<IAutomationScriptParameterViewModel> Parameters
    {
        get => _parameters;
        set => SetProperty(ref _parameters, value);
    }

    private IAutomationScriptDescription _selectedScriptDescription;
    public IAutomationScriptDescription ScriptDescription
    {
        get => _selectedScriptDescription;
        set => SetProperty(ref _selectedScriptDescription, value);
    }

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
