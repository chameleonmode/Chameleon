using AutoMapper;
using Chameleon.Interfaces.Settings;

namespace Chameleon.Infrastructure.UserSettings
{
    public class UserSettingsMapProfile : Profile
    {
        public UserSettingsMapProfile()
        {
            UserProfileDtoMap();
            //CreateUserProfileDtoMap();
        }

        private void UserProfileDtoMap()
        {
            var map = CreateMap<UserSettingsDto, IUserSetting>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                .ForMember(model => model.SmsPvaApiKey, options => options.MapFrom(dto => dto.SmsPvaApiKey));

            map.ForAllOtherMembers(options => options.Ignore());

            map.ReverseMap()
                .ForAllOtherMembers(options => options.Ignore());
        }

        //private void CreateUserProfileDtoMap()
        //{
        //    CreateMap<IUserSetting, CreateUserSettingsDto>()
        //        .ForMember(model => model.SmsPvaApiKey, options => options.MapFrom(dto => dto.SmsPvaApiKey))
        //        .ForAllOtherMembers(options => options.Ignore());
        //}
    }
}
