using AutoMapper;
using Chameleon.Interfaces.OutReach;

namespace Chameleon.Infrastructure.OutReach.Api.Dto
{
    public class OutReachTemplateMapProfile : Profile
    {
        public OutReachTemplateMapProfile()
        {
            UserProfileDtoMap();
            CreateUserProfileDtoMap();
        }

        private void UserProfileDtoMap()
        {
            var map = CreateMap<OutReachTemplateDto, IOutReachTemplate>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                .ForMember(model => model.Name, options => options.MapFrom(dto => dto.Name))
                .ForMember(model => model.ContactEmail, options => options.MapFrom(dto => dto.ContactEmail))
                .ForMember(model => model.ContactName, options => options.MapFrom(dto => dto.ContactName))
                .ForMember(model => model.Subject, options => options.MapFrom(dto=> dto.Subject))
                .ForMember(model => model.Content, options => options.MapFrom(dto => dto.Content));

            map.ReverseMap();
        }

        private void CreateUserProfileDtoMap()
        {
            CreateMap<IOutReachTemplate, CreateOutReachTemplateDto>()
                .ForMember(model => model.Subject, options => options.MapFrom(dto => dto.Subject))
                .ForMember(model => model.Name, options => options.MapFrom(dto => dto.Name))
                .ForMember(model => model.Content, options => options.MapFrom(dto => dto.Content))
                .ForAllOtherMembers(options => options.Ignore());
        }
    }
}
