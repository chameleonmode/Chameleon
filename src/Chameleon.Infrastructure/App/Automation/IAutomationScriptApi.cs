using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.App.Automation.DTOs;
using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Repository;
using System.Collections.Generic;

namespace Chameleon.Infrastructure.App.Automation;

public interface IAutomationScriptApi
     : IApiLayer<AutomationScriptDto, int, GetAllRequestDto, AutomationScriptDto>
     , ISingletonDependency
{
    void UpdateParameter(IAutomationScriptParameter param);
    void SetParametersValue(IList<AutomationScriptParameterValueDto> valueDtos);
    IList<AutomationScriptDescriptionDto> GetAllScriptDescription(object query = null);
}