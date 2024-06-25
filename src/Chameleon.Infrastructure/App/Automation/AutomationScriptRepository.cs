using AutoMapper;
using Chameleon.Domain.Entities.Automation;
using Chameleon.Infrastructure.App.Automation.DTOs;
using Chameleon.Infrastructure.Repositories;
using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.App.Automation.Repositories;
using Chameleon.Interfaces.Repository;
using System.Collections.Generic;

namespace Chameleon.Infrastructure.App.Automation;
public class AutomationScriptRepository
: Repository<AutomationScript,
            IAutomationScript,
            int,
            AutomationScriptDto,
            GetAllRequestDto>
        , IAutomationScriptRepository
{
    private new readonly IMapper _mapper;
    private readonly IAutomationScriptApi _client;

    public AutomationScriptRepository(
        IMapper mapper,
        IAutomationScriptApi apiClient,
        IEventAggregator eventAggregator
        )
        : base(mapper, apiClient, eventAggregator)
    {
        _client = apiClient;
        _mapper = mapper;
    }

    public void UpdateParameter(IAutomationScriptParameter param)
    {
        _client.UpdateParameter(param);
    }

    public void SetParametersValue(IList<IAutomationParameterValue> values)
    {
        var valuesDtos = new List<AutomationScriptParameterValueDto>();
        var parameters = _mapper.Map(values, valuesDtos);

        _client.SetParametersValue(parameters);

        TriggerEntityEvent<SavedEntityEvent>();
    }

    public IList<IAutomationScriptDescription> GetAllScriptDescription()
    {
        var scriptDtos = _client.GetAllScriptDescription();
        var scripts = new List<IAutomationScriptDescription>();

        foreach (var script in scriptDtos)
        {
            var scriptDtoMapped = new AutomationScriptDescription
            {
                Id = script.Id,
                Title = script.Title,
                Description = script.Description,
                Parameters = new List<IAutomationParameterValue>()
            };

            foreach (var parameter in script.Parameters)
            {
                var parameterDtoMapped = new AutomationParameterValue
                {
                    Id = parameter.Id,
                    Name = parameter.Name,
                    Value = parameter.Value,
                    ParameterId = parameter.Id,
                };
                scriptDtoMapped.Parameters.Add(parameterDtoMapped);
            }
            scripts.Add(scriptDtoMapped);
        }

        return scripts;
    }

    public override IAutomationScript Get(int id)
    {
        var scriptDto = _client.Get(id);
        var scripts = new AutomationScript();
        var map = _mapper.Map(scriptDto, scripts);

        return map;
    }

    public string GetScriptBody(int id)
    {
        var scriptBodyDto = _client.GetScriptBody(id);

        return scriptBodyDto;
    }
}
