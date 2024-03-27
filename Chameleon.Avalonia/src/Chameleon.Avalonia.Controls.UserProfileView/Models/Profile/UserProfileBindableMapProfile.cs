using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.Interfaces.WordPress;
using Chameleon.Interfaces.YouTube;
using AutoMapper;
using Chameleon.Avalonia.Controls.UserProfileView.Models.WebBrowser;
using Chameleon.Avalonia.Controls.UserProfileView.Models.ProxySettings;
using Chameleon.Avalonia.Controls.UserProfileView.Models.Youtube;
using Chameleon.Avalonia.Controls.UserProfileView.Models.WordPress;
using Chameleon.Interfaces.Entities;
using Chameleon.Domain.Entities;

namespace Chameleon.Avalonia.Controls.UserProfileView.Models.Profile;

public class UserProfileBindableMapProfile : AutoMapper.Profile
{
    public UserProfileBindableMapProfile()
    {
        CreateMap<IWebBrowserSettings, WebBrowserSettingsBindable>()
            .ForMember(bindable => bindable.WebRTC, options => options.MapFrom(entity => entity.WebRTC))
            .ForMember(bindable => bindable.WebGL, options => options.MapFrom(entity => entity.WebGL))
            .ForMember(bindable => bindable.Tracking, options => options.MapFrom(entity => entity.Tracking))
            .ForMember(bindable => bindable.Flash, options => options.MapFrom(entity => entity.Flash))
            .ForMember(bindable => bindable.Canvas, options => options.MapFrom(entity => entity.Canvas))
            .ForMember(bindable => bindable.UserAgentId, options => options.MapFrom(entity => entity.UserAgentId))
            .ForAllOtherMembers(opts => opts.Ignore())
            ;

        CreateMap<IProxySettings, ProxySettingsBindable>()
            .ForMember(bindable => bindable.Host, options => options.MapFrom(entity => entity.Host))
            .ForMember(bindable => bindable.Port, options => options.MapFrom(entity => entity.Port))
            .ForMember(bindable => bindable.UserName, options => options.MapFrom(entity => entity.UserName))
            .ForMember(bindable => bindable.Password, options => options.MapFrom(entity => entity.Password))
            .ForAllOtherMembers(opts => opts.Ignore())
            ;

        CreateMap<IYouTubeSettings, YoutubeSettingsBindable>()
            .ForMember(bindable => bindable.ApiKey, options => options.MapFrom(entity => entity.ApiKey))
            .ForMember(bindable => bindable.ClientId, options => options.MapFrom(entity => entity.ClientId))
            .ForMember(bindable => bindable.ClientSecret, options => options.MapFrom(entity => entity.ClientSecret))
            .ForAllOtherMembers(opts => opts.Ignore())
            ;

        CreateMap<IWordPressSettings, WordPressSettingsBindable>()
            .ForMember(bindable => bindable.BaseUrl, options => options.MapFrom(entity => entity.BaseUrl))
            .ForMember(bindable => bindable.Username, options => options.MapFrom(entity => entity.Username))
            .ForMember(bindable => bindable.Password, options => options.MapFrom(entity => entity.Password))
            .ForAllOtherMembers(opts => opts.Ignore())
        ;

        //var map1 = CreateMap<ValueTuple<IUserProfile, IUserProfileInfo, IEntity, IEntity<int>>, UserProfileBindable>()
        //.ForMember(
        //    dest => dest.Id,
        //    src => src.MapFrom(x => x.Item4.Id))
        //.ForMember(
        //    dest => dest.Notes,
        //    src => src.MapFrom(x => x.Item2.Notes))
        //.ForMember(
        //    dest => dest.IsFavourite,
        //    src => src.MapFrom(x => x.Item2.IsFavourite))
        //.ForMember(
        //    dest => dest.Title,
        //    src => src.MapFrom(x => x.Item2.Title))
        //.ForMember(
        //    dest => dest.FolderId,
        //    src => src.MapFrom(x => x.Item2.FolderId));
        //map1.ForAllOtherMembers(opts => opts.Ignore());
        //map1.ReverseMap();
        var map = CreateMap<UserProfile, UserProfileBindable>()
            .ForMember(bindable => bindable.Id, options => options.MapFrom(entity => entity.Id))
            .ForMember(bindable => bindable.Notes, options => options.MapFrom(entity => entity.Notes))
            .ForMember(bindable => bindable.IsFavourite, options => options.MapFrom(entity => entity.IsFavourite))
            .ForMember(bindable => bindable.Title, options => options.MapFrom(entity => entity.Title))
            .ForMember(bindable => bindable.FolderId, options => options.MapFrom(entity => entity.FolderId))
           .ForMember(bindable => bindable.WebBrowser, options => options.MapFrom(entity => entity.WebBrowser))
           .ForMember(bindable => bindable.Proxy, options => options.MapFrom(entity => entity.Proxy))
           //.ForMember(bindable => bindable.YoutubeSettings, options => options.MapFrom(entity => entity.YoutubeSettings))
           //.ForMember(bindable => bindable.WordPressSettings, options => options.MapFrom(entity => entity.WordPressSettings))
            ;

        map.ForAllOtherMembers(opts => opts.Ignore());

        map.ReverseMap();
   //     var map1 = CreateMap<UserProfileBindable, UserProfile>()
   // .ForMember(bindable => bindable.Id, options => options.MapFrom(entity => entity.Id))
   // .ForMember(bindable => bindable.Notes, options => options.MapFrom(entity => entity.Notes))
   // .ForMember(bindable => bindable.IsFavourite, options => options.MapFrom(entity => entity.IsFavourite))
   // .ForMember(bindable => bindable.Title, options => options.MapFrom(entity => entity.Title))
   // .ForMember(bindable => bindable.FolderId, options => options.MapFrom(entity => entity.FolderId))
   //.ForMember(bindable => bindable.WebBrowser, options => options.MapFrom(entity => entity.WebBrowser))
   //.ForMember(bindable => bindable.Proxy, options => options.MapFrom(entity => entity.Proxy))
   //.ForMember(bindable => bindable.YoutubeSettings, options => options.MapFrom(entity => entity.YoutubeSettings))
   //.ForMember(bindable => bindable.WordPressSettings, options => options.MapFrom(entity => entity.WordPressSettings))
   // ;

   //     map1.ForAllOtherMembers(opts => opts.Ignore());

   //     map1.ReverseMap();

    }
}
