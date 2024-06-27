using AutoMapper;
using Chameleon.Domain.Entities.Automation;
using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.App.Automation.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Chameleon.Infrastructure.App.Automation.DTOs;
public class AutomationScriptMapProfile
    : Profile
{
    public AutomationScriptMapProfile()
    {
        AutomationScriptDtoMap();
        AutomationScriptParameterDtoMap();
        AutomationScriptDescriptionMap();
        AutomationScriptParameterValueDtoMap();
        IAutomationParameterValue();
    }

    private void AutomationScriptDtoMap()
    {
        var map = CreateMap<AutomationScriptDto, IAutomationScript>()
            .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
            .ForMember(model => model.Title, options => options.MapFrom(dto => dto.Title))
            .ForMember(model => model.Description, options => options.MapFrom(dto => dto.Description))
            .ForMember(model => model.Parameters, options => options.MapFrom(dto => dto.Parameters));

        map.ForAllOtherMembers(options => options.Ignore());

        map.ReverseMap()
            .ForAllOtherMembers(options => options.Ignore());
    }

    private void AutomationScriptParameterDtoMap()
    {
        var map = CreateMap<AutomationScriptParameterDto, IAutomationScriptParameter>()
            .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
            .ForMember(model => model.ScriptId, options => options.MapFrom(dto => dto.ScriptId))
            .ForMember(model => model.Name, options => options.MapFrom(dto => dto.Name))
            .ForMember(model => model.Value, options => options.MapFrom(dto => dto.Value));

        map.ForAllOtherMembers(options => options.Ignore());

        map.ReverseMap()
            .ForAllOtherMembers(options => options.Ignore());
    }

    private void IAutomationParameterValue()
    {
        var map = CreateMap<IAutomationParameterValue, AutomationScriptParameterValueDto>()
            .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
            .ForMember(model => model.Value, options => options.MapFrom(dto => dto.Value))
            .ForMember(model => model.ParameterId, options => options.MapFrom(dto => dto.ParameterId))
            .ForMember(model => model.Name, options => options.MapFrom(dto => dto.Name));

        map.ForAllOtherMembers(options => options.Ignore());

        map.ReverseMap()
            .ForAllOtherMembers(options => options.Ignore());
    }

    private void AutomationScriptParameterValueDtoMap()
    {
        var map = CreateMap<AutomationScriptParameterValueDto, AutomationParameterValue>()
            .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
            .ForMember(model => model.Name, options => options.MapFrom(dto => dto.Name))
            .ForMember(model => model.Value, options => options.MapFrom(dto => dto.Value))
            .ForMember(model => model.ParameterId, options => options.MapFrom(dto => dto.ParameterId));

        map.ForAllOtherMembers(options => options.Ignore());

        map.ReverseMap()
            .ForAllOtherMembers(options => options.Ignore());
    }

    private void AutomationScriptDescriptionMap()
    {
        var map = CreateMap<AutomationScriptDescriptionDto, AutomationScriptDescription>()
            .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
            .ForMember(model => model.Title, options => options.MapFrom(dto => dto.Title))
            .ForMember(model => model.Description, options => options.MapFrom(dto => dto.Description))
            .ForMember(model => model.Parameters, options => options.MapFrom(dto => dto.Parameters));

        map.ForAllOtherMembers(options => options.Ignore());

        map.ReverseMap()
            .ForAllOtherMembers(options => options.Ignore());
    }
}
