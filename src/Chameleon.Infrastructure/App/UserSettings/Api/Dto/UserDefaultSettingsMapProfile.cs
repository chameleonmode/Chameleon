using AutoMapper;
using Chameleon.Interfaces.Settings;

namespace Chameleon.Infrastructure.UserSettings
{
    public class UserDefaultSettingsMapProfile : Profile
    {
        public UserDefaultSettingsMapProfile()
        {
            UserProfileDtoMap();
            CreateUserProfileDtoMap();
        }

        private void UserProfileDtoMap()
        {
            var map = CreateMap<UserDefaultSettingsDto, IUserDefaultSetting>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                .ForMember(model => model.DefaultUrl, options => options.MapFrom(dto => dto.DefaultUrl))
                ;

            map.ForAllOtherMembers(options => options.Ignore());

            map.ReverseMap()
                .ForAllOtherMembers(options => options.Ignore());
        }

        private void CreateUserProfileDtoMap()
        {
            CreateMap<IUserDefaultSetting, CreateUserDefaultSettingsDto>()
                .ForMember(model => model.DefaultUrl, options => options.MapFrom(dto => dto.DefaultUrl))
                .ForAllOtherMembers(options => options.Ignore());
        }
    }
}
