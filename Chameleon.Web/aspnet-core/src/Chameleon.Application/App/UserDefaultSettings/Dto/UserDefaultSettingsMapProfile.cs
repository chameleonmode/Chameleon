using AutoMapper;
using Chameleon.App.Entities;

namespace Chameleon.App
{
    public class UserDefaultSettingsMapProfile : AutoMapper.Profile
    {
        public UserDefaultSettingsMapProfile() 
        { 
              CreateEntityDtoMap<UserDefaultSettingsDto>()
                .ReverseMap();

              CreateBaseDtoMap<UserDefaultSettingsBaseDto>()
                .ReverseMap();

              CreateBaseDtoMap<CreateUserDefaultSettingsDto>();
              CreateEntityDtoMap<UpdateUserDefaultSettingsDto>();
        }

        private IMappingExpression<TDto, UserDefaultSettings> CreateEntityDtoMap<TDto>()
            where TDto : UserDefaultSettingsEntityDto
        {
            return CreateBaseDtoMap<TDto>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id));
        }

        private IMappingExpression<TDto, UserDefaultSettings> CreateBaseDtoMap<TDto>()
            where TDto : UserDefaultSettingsBaseDto
        {
            return CreateMap<TDto, UserDefaultSettings>()
                .ForMember(model => model.DefaultUrl, options => options.MapFrom(dto => dto.DefaultUrl));
        }
    }
}
