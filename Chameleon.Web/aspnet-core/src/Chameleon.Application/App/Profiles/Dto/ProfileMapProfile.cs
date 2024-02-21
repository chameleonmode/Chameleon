using Chameleon.App.Entities;

namespace Chameleon.App.Dto
{
    public class ProfileMapProfile : AutoMapper.Profile
    {
        public ProfileMapProfile()
        {
            CreateWordPressSettingsMap()
                .ReverseMap()
                ;

            CreateEntityDtoMap<ProfileDto>()
                .ForMember(model => model.CreatorUserId, options => options.MapFrom(dto => dto.CreatorUserId))
                .ReverseMap()
                ;

            CreateBaseDtoMap<CreateProfileDto>();
            CreateEntityDtoMap<UpdateProfileDto>();

            CreateMap<Profile, ProfileInfoDto>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                .ForMember(model => model.Title, options => options.MapFrom(dto => dto.Title))
                .ForMember(model => model.CreatorUserId, options => options.MapFrom(dto => dto.CreatorUserId))
                .ForAllOtherMembers(options => options.Ignore())
                ;
        }

        private AutoMapper.IMappingExpression<TDto, Profile> CreateEntityDtoMap<TDto>()
            where TDto : ProfileEntityDto
        {
            return CreateBaseDtoMap<TDto>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                ;
        }
        
        private AutoMapper.IMappingExpression<WordPressSettingsDto, WordPressSettings> CreateWordPressSettingsMap()
        {
            return CreateMap<WordPressSettingsDto, WordPressSettings>()
                .ForMember(model => model.BaseUrl, options => options.MapFrom(dto => dto.BaseUrl))
                .ForMember(model => model.Username, options => options.MapFrom(dto => dto.Username))
                .ForMember(model => model.Password, options => options.MapFrom(dto => dto.Password))
                ;
        }

        private AutoMapper.IMappingExpression<TDto, Profile> CreateBaseDtoMap<TDto>()
            where TDto : ProfileBaseDto
        {
            return CreateMap<TDto, Profile>()
                .ForMember(model => model.Title, options => options.MapFrom(dto => dto.Title))
                .ForMember(model => model.Notes, options => options.MapFrom(dto => dto.Notes))
                .ForMember(model => model.IsFavourite, options => options.MapFrom(dto => dto.IsFavourite))
                .ForMember(model => model.FolderId, options => options.MapFrom(dto => dto.FolderId))
                .ForMember(model => model.Proxy, options => options.MapFrom(dto => dto.Proxy))
                .ForMember(model => model.WebBrowserSetting, options => options.MapFrom(dto => dto.WebBrowser))
                .ForMember(model => model.LimitCache, options => options.MapFrom(dto => dto.LimitCache))
                .ForMember(model => model.YoutubeApiKey, options => options.MapFrom(dto => dto.YoutubeApiKey))
                .ForMember(model => model.YoutubeClientId, options => options.MapFrom(dto => dto.YoutubeClientId))
                .ForMember(model => model.YoutubeClientSecret, options => options.MapFrom(dto => dto.YoutubeClientSecret))
                .ForMember(model => model.WordPressSettings, options => options.MapFrom(dto => dto.WordPressSettings))
                ;
        }
    }
}
