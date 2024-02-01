using AutoMapper;
using Chameleon.Domain.Entities.Assistants;
using Chameleon.Interfaces.App.Assistants;

namespace Chameleon.Infrastructure.App.Assistants.AssistantProfilePermissions.Api.Dto
{
    public class AssistantProfilePermissionMapProfile : Profile
    {
        public AssistantProfilePermissionMapProfile()
        {
            AssistantProfilePermissionDtoMap();
        }

        private void AssistantProfilePermissionDtoMap()
        {
            CreateMap<AssistantProfilePermissionDto, IAssistantProfilePermission>()
                .ConstructUsing(dto => new AssistantProfilePermission())
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                .ForMember(model => model.PermissionName, options => options.MapFrom(dto => dto.PermissionName))
                .ForMember(model => model.DisplayName, options => options.MapFrom(dto => dto.DisplayName))
                .ForMember(model => model.IsGranted, options => options.MapFrom(dto => dto.IsGranted))
                .ForMember(model => model.ProfileAssistantId, options => options.MapFrom(dto => dto.ProfileAssistantId))
                .ForAllOtherMembers(options => options.Ignore());

            CreateMap<IAssistantProfilePermission, AssistantProfilePermissionDto>()
                .ForMember(dto => dto.Id, options => options.MapFrom(model => model.Id))
                .ForMember(dto => dto.PermissionName, options => options.MapFrom(model => model.PermissionName))
                .ForMember(dto => dto.DisplayName, options => options.MapFrom(model => model.DisplayName))
                .ForMember(dto => dto.IsGranted, options => options.MapFrom(model => model.IsGranted))
                .ForMember(dto => dto.ProfileAssistantId, options => options.MapFrom(model => model.ProfileAssistantId))
                .ForAllOtherMembers(options => options.Ignore());
        }
    }
}
