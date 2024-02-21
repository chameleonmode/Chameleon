using AutoMapper;
using Chameleon.App.Entities;

namespace Chameleon.App
{
    public class UserSettingsMapProfile : AutoMapper.Profile
    {
        public UserSettingsMapProfile()
        {
            CreateEntityDtoMap<UserSettingsDto>()
              .ReverseMap();

            CreateBaseDtoMap<UserSettingsBaseDto>()
              .ReverseMap();

            CreateBaseDtoMap<CreateUserSettingsDto>();
            CreateEntityDtoMap<UpdateUserSettingsDto>();
        }

        private IMappingExpression<TDto, UserSettings> CreateEntityDtoMap<TDto>()
            where TDto : UserSettingsEntityDto
        {
            return CreateBaseDtoMap<TDto>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id));
        }

        private IMappingExpression<TDto, UserSettings> CreateBaseDtoMap<TDto>()
            where TDto : UserSettingsBaseDto
        {
            return CreateMap<TDto, UserSettings>()
                .ForMember(model => model.SmsPvaApiKey, options => options.MapFrom(dto => dto.SmsPvaApiKey));
        }
    }
}
