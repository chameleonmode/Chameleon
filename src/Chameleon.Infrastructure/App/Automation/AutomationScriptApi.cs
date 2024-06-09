using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.App.Automation.DTOs;
using Chameleon.Interfaces.Api;
using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.Repository;
using System.Collections;
using System.Collections.Generic;

namespace Chameleon.Infrastructure.App.Automation;
public class AutomationScriptApi
    : ApiLayer<AutomationScriptDto, int, GetAllRequestDto, AutomationScriptDto>
    , IAutomationScriptApi
{
    public AutomationScriptApi(
            IApiClient apiClient
            ) : base(apiClient, "automation")
    {
    }

    public void UpdateParameter(IAutomationScriptParameter param)
    {
        _apiClient.Put(GetEndpointUrl("UpdateParameter"), param);
    }

    public void SetParametersValue(IList<AutomationScriptParameterValueDto> valueDtos)
    {
        _apiClient.Post(GetEndpointUrl("SetParametersValue"), valueDtos);
    }

    public IList<AutomationScriptDescriptionDto> GetAllScriptDescription(object query = null)
    {
        if (query == null)
        {
            query = new PagedResultRequestDto
            {
                MaxResultCount = int.MaxValue
            };
        }

        var scriptDtos = _apiClient.Get<AutomationScriptDescriptionDto[]>(GetEndpointUrl("GetAllScriptDescription"), query);
        return scriptDtos;
    }

    public string GetScriptBody(int id)
    {
        var query = new { Id = id };
        var scriptDtos = _apiClient.Get<AutomationScriptBodyDto>(GetEndpointUrl("GetScriptBody"), query);
        return scriptDtos.Script;
    }
}
