using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.WebBrowser;

namespace Chameleon.Infrastructure.WebBrowsers
{
    public class WebBrowserUserAgentMapProfile : Profile
    {
        public WebBrowserUserAgentMapProfile()
        {
            UserAgentDtoMap();
            UserAgentInsertDtoMap();
        }

        private void UserAgentDtoMap()
        {
            CreateMap<WebBrowserUserAgentDto, WebBrowserUserAgent>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                .ForMember(model => model.Name, options => options.MapFrom(dto => dto.Name))
                .ForMember(model => model.Value, options => options.MapFrom(dto => dto.Value))
                ;

            CreateMap<IWebBrowserUserAgent, WebBrowserUserAgentDto>()
                .ForMember(dto => dto.Id, options => options.MapFrom(model => model.Id))
                .ForMember(dto => dto.Name, options => options.MapFrom(model => model.Name))
                .ForMember(dto => dto.Value, options => options.MapFrom(model => model.Value))
                ;
        }

        private void UserAgentInsertDtoMap()
        {
            CreateMap<IWebBrowserUserAgent, CreateWebBrowserUserAgentDto>()
                .ForMember(model => model.Name, options => options.MapFrom(dto => dto.Name))
                .ForMember(model => model.Value, options => options.MapFrom(dto => dto.Value))
                .ForAllOtherMembers(x => x.Ignore());
        }
    }
}
