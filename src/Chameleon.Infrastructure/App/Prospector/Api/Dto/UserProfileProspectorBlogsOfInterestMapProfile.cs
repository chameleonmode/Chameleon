using AutoMapper;
using Chameleon.Interfaces.App.Prospector;

namespace Chameleon.Infrastructure.Prospector.Api.Dto
{
    public class UserProfileProspectorBlogsOfInterestMapProfile : Profile
    {
        public UserProfileProspectorBlogsOfInterestMapProfile()
        {
            DtoMap();
            CreateDtoMap();
        }

        private void DtoMap()
        {
            var map = CreateMap<UserProfileProspectorBlogsOfInterestDto, IUserProfileProspectorBlogsOfInterest>()
               .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
               .ForMember(model => model.Name, options => options.MapFrom(dto => dto.Name))
               .ForMember(model => model.Value, options => options.MapFrom(dto => dto.Value))
               .ForMember(model => model.Type, options => options.MapFrom(dto => dto.Type))
               .ForMember(model => model.ProfileId, options => options.MapFrom(dto => dto.ProfileId));

            map.ReverseMap();
        }

        private void CreateDtoMap()
        {
            CreateMap<IUserProfileProspectorBlogsOfInterest, CreateUserProfileProspectorBlogsOfInterestDto>()
               .ForMember(model => model.Name, options => options.MapFrom(dto => dto.Name))
               .ForMember(model => model.Value, options => options.MapFrom(dto => dto.Value))
               .ForMember(model => model.Type, options => options.MapFrom(dto => dto.Type))
               .ForMember(model => model.ProfileId, options => options.MapFrom(dto => dto.ProfileId));
        }
    }
}
