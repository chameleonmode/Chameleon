using AutoMapper;
using Chameleon.App.Entities;

namespace Chameleon.App
{
    public class AppLoggerMapProfile : AutoMapper.Profile
    {
        public AppLoggerMapProfile()
        {
            CreateEntityDtoMap<AppLoggerDto>()
              .ReverseMap();

            CreateBaseDtoMap<AppLoggerBaseDto>()
              .ReverseMap();

            CreateBaseDtoMap<CreateAppLoggerDto>();
            CreateEntityDtoMap<UpdateAppLoggerDto>();
        }

        private IMappingExpression<TDto, AppLogger> CreateEntityDtoMap<TDto>()
            where TDto : AppLoggerEntityDto
        {
            return CreateBaseDtoMap<TDto>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id));
        }

        private IMappingExpression<TDto, AppLogger> CreateBaseDtoMap<TDto>()
            where TDto : AppLoggerBaseDto
        {
            return CreateMap<TDto, AppLogger>()
                .ForMember(model => model.UserId, options => options.MapFrom(dto => dto.UserId))
                .ForMember(model => model.Message, options => options.MapFrom(dto => dto.Message))
                .ForMember(model => model.UserName, options => options.MapFrom(dto => dto.UserName))
                .ForMember(model => model.CreationTime, options => options.MapFrom(dto => dto.CreationTime))
                .ForMember(model => model.AppLoggerType, options => options.MapFrom(dto => dto.AppLoggerType.ToString()))
                ;
        }
    }
}
