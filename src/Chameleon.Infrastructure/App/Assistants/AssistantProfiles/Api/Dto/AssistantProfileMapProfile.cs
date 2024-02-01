using AutoMapper;
using Chameleon.Domain.Entities.Assistants;
using Chameleon.Interfaces.Assistants;

namespace Chameleon.Infrastructure.App.Assistants.AssistantProfiles.Api.Dto
{
    public class AssistantProfileMapProfile : Profile
    {
        public AssistantProfileMapProfile()
        {
            AssistantProfileDtoMap();
        }

        private void AssistantProfileDtoMap()
        {
            CreateMap<AssistantProfileDto, IAssistantProfile>()
                .ConstructUsing(dto => new AssistantProfile())
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                .ForMember(model => model.ProfileId, options => options.MapFrom(dto => dto.ProfileId))
                .ForMember(model => model.ProfileName, options => options.MapFrom(dto => dto.ProfileName))
                .ForAllOtherMembers(options => options.Ignore());

            CreateMap<IAssistantProfile, AssistantProfileDto>()
                .ForMember(dto => dto.Id, options => options.MapFrom(model => model.Id))
                .ForMember(dto => dto.ProfileId, options => options.MapFrom(model => model.ProfileId))
                .ForMember(dto => dto.ProfileName, options => options.MapFrom(model => model.ProfileName))
                .ForAllOtherMembers(options => options.Ignore());
        }
    }
}
