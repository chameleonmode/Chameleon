using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.WebBrowser;

namespace Chameleon.Infrastructure.UserProfiles.Api.Dto.WebBrowser
{
    public class WebBrowserMapProfile : Profile
    {
        public WebBrowserMapProfile()
        {
            CreateMap<WebBrowserDto, IWebBrowserSettings>()
                .ConstructUsing(dto => new WebBrowserSettings())
                .ForMember(model => model.WebRTC, options => options.MapFrom(dto => dto.WebRTC))
                .ForMember(model => model.WebGL, options => options.MapFrom(dto => dto.WebGL))
                .ForMember(model => model.Tracking, options => options.MapFrom(dto => dto.Tracking))
                .ForMember(model => model.Flash, options => options.MapFrom(dto => dto.Flash))
                .ForMember(model => model.Canvas, options => options.MapFrom(dto => dto.Canvas))
                .ForMember(model => model.UserAgentId, options => options.MapFrom(dto => dto.UserAgentId))
                .ForAllOtherMembers(options => options.Ignore())
                ;

            CreateMap<IWebBrowserSettings, WebBrowserDto>()
                .ForMember(dto => dto.WebRTC, options => options.MapFrom(model => model.WebRTC))
                .ForMember(dto => dto.WebGL, options => options.MapFrom(model => model.WebGL))
                .ForMember(dto => dto.Tracking, options => options.MapFrom(model => model.Tracking))
                .ForMember(dto => dto.Flash, options => options.MapFrom(model => model.Flash))
                .ForMember(dto => dto.Canvas, options => options.MapFrom(model => model.Canvas))
                .ForMember(dto => dto.UserAgentId, options => options.MapFrom(model => model.UserAgentId))
                .ForAllOtherMembers(options => options.Ignore())
                ;
        }
    }
}
