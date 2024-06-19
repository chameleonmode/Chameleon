using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.App.Automation.Repositories;
using Chameleon.Interfaces.App.Automation.Services;
using System;
using System.Collections.Generic;

namespace Chameleon.Infrastructure.App.Automation;
public class AutomationService
    : IAutomationService
{
    private readonly IAutomationScriptRepository _automationRepository;

    public AutomationService(
        IAutomationScriptRepository repository
        )
    {
        _automationRepository = repository;

        InitScripts();
    }

    private Lazy<IList<IAutomationScriptDescription>> _scripts;
    private IList<IAutomationScriptDescription> Scripts => _scripts.Value;

    public IList<IAutomationScriptDescription> GetAll()
    {
        return Scripts;
    }

    public void UpdateParameter(IAutomationScriptParameter param)
    {
        _automationRepository.UpdateParameter(param);
    }

    public void SetParametersValue(IList<IAutomationParameterValue> values)
    {
        _automationRepository.SetParametersValue(values);
    }

    private void InitScripts()
    {
        _scripts = new Lazy<IList<IAutomationScriptDescription>>(() => GetScripts(), true);
    }

    private IList<IAutomationScriptDescription> GetScripts()
    {
        var entities = _automationRepository.GetAllScriptDescription();
        var response = new List<IAutomationScriptDescription>(entities);

        return response;
    }
}
