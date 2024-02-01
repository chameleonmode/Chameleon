using AutoMapper;
using Chameleon.Interfaces.ExceptionOptions;

namespace Chameleon.Infrastructure.ExceptionOptions
{
    public class AppLoggerMapProfile : Profile
    {
        public AppLoggerMapProfile()
        {
            UserProfileDtoMap();
            CreateUserProfileDtoMap();
        }

        private void UserProfileDtoMap()
        {
            var map = CreateMap<AppLoggerDto, IAppLogger>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                .ForMember(model => model.Message, options => options.MapFrom(dto => dto.Message))
                .ForMember(model => model.AppLoggerType, options => options.MapFrom(dto => dto.AppLoggerType.ToString()))
                ;

            map.ForAllOtherMembers(options => options.Ignore());

            map.ReverseMap()
                .ForAllOtherMembers(options => options.Ignore());
        }

        private void CreateUserProfileDtoMap()
        {
            CreateMap<IAppLogger, CreateAppLoggerDto>()
                .ForMember(model => model.Message, options => options.MapFrom(dto => dto.Message))
                .ForMember(model => model.AppLoggerType, options => options.MapFrom(dto => dto.AppLoggerType))
                .ForAllOtherMembers(options => options.Ignore());
        }
    }
}
