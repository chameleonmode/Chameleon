using AutoMapper;
using Chameleon.Avalonia.Controls.Automation.ViewModels;
using Chameleon.Core.Extensions;
using Chameleon.CT.Common.Base;
using Chameleon.Domain.Entities.Automation;
using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.App.Automation.Services;
using Chameleon.Interfaces.App.Automation.ViewModels;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Repository;
using System.Reflection.Metadata;

namespace Chameleon.Avalonia.Controls.Automation.Views.ViewModels;

public class AddScriptParametersPopupViewModel
    : ObservableObjectBase
    , IAddScriptParametersPopupViewModel
{
    private Dictionary<IAutomationParameterValue, object> _originalParameterValues;
    private readonly IAutomationService _automationService;

    public AddScriptParametersPopupViewModel(
        IAutomationService automationService
        )
    {
        _automationService = automationService;
    }

    private IAutomationScriptDescription _scriptDescription;
    public IAutomationScriptDescription ScriptDescription
    {
        get => _scriptDescription;
        set
        {
            SetProperty(ref _scriptDescription, value);
            UpdateOriginalParameterValues();
        }
    }

    private void UndoParameters(IList<IAutomationParameterValue> parameters)
    {
        foreach (var parameter in parameters)
        {
            if (_originalParameterValues.ContainsKey(parameter))
            {
                parameter.Value = (string)_originalParameterValues[parameter];
            }
        }
    }

    private void UpdateOriginalParameterValues()
    {
        _originalParameterValues = new Dictionary<IAutomationParameterValue, object>();
        foreach (var parameter in ScriptDescription.Parameters)
        {
            _originalParameterValues.Add(parameter, parameter.Value);
        }
    }

    public void OnDialogClosing(IContentDialogResult result)
    {
        if (result == IContentDialogResult.Primary)
        {
            SaveParameters();
        }
        else
        {
            UndoParameters(ScriptDescription.Parameters);
        }
    }

    private void SaveParameters()
    {
        _automationService.SetParametersValue(ScriptDescription.Parameters);
    }
}
