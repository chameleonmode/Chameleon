using AutoMapper;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.WordPress;

namespace Chameleon.Infrastructure.Profiles
{
    public class UserProfileMapProfile : Profile
    {
        public UserProfileMapProfile()
        {
            UserProfileDtoMap();
            CreateUserProfileDtoMap();
        }

        private void UserProfileDtoMap()
        {
            var map = CreateMap<UserProfileDto, IUserProfile>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                .ForMember(model => model.Title, options => options.MapFrom(dto => dto.Title))
                .ForMember(model => model.FolderId, options => options.MapFrom(dto => dto.FolderId))
                .ForMember(model => model.CreatorUserId, options => options.MapFrom(dto => dto.CreatorUserId))
                .ForMember(model => model.Notes, options => options.MapFrom(dto => dto.Notes))
                .ForMember(model => model.IsFavourite, options => options.MapFrom(dto => dto.IsFavourite))
                .ForMember(model => model.WebBrowser, options => options.MapFrom(dto => dto.WebBrowser))
                .ForMember(model => model.Proxy, options => options.MapFrom(dto => dto.Proxy))
                .ForMember(model => model.LimitCache, options => options.MapFrom(dto => dto.LimitCache))
                .ForMember(model => model.WordPressSettings, options => options.MapFrom(dto => dto.WordPressSettings))
                .ForPath(model => model.YoutubeSettings.ApiKey, options => options.MapFrom(dto => dto.YoutubeApiKey))
                .ForPath(model => model.YoutubeSettings.ClientId, options => options.MapFrom(dto => dto.YoutubeClientId))
                .ForPath(model => model.YoutubeSettings.ClientSecret, options => options.MapFrom(dto => dto.YoutubeClientSecret))
                .ForMember(model => model.CreatorUserId, options => options.UseDestinationValue())
                ;

            map.ForAllOtherMembers(options => options.Ignore());

            map.ReverseMap()
                .ForAllOtherMembers(options => options.Ignore());
        }

        private void CreateUserProfileDtoMap()
        {
            CreateMap<IUserProfile, CreateUserProfileDto>()
                .ForMember(model => model.Title, options => options.MapFrom(dto => dto.Title))
                .ForMember(model => model.FolderId, options => options.MapFrom(dto => dto.FolderId))
                .ForMember(model => model.WebBrowser, options => options.MapFrom(dto => dto.WebBrowser))
                .ForMember(model => model.Proxy, options => options.MapFrom(dto => dto.Proxy))
                .ForAllOtherMembers(options => options.Ignore());
        }
    }
}
