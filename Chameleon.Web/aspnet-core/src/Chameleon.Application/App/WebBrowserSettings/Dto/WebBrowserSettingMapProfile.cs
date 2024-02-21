using AutoMapper;
using Chameleon.App.Entities;

namespace Chameleon.App.Dto
{
    public class WebBrowserSettingMapProfile : AutoMapper.Profile
    {
        public WebBrowserSettingMapProfile()
        {
            CreateBaseDtoMap<WebBrowserSettingDto>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                .ReverseMap()
                ;

            CreateBaseDtoMap<WebBrowserSettingBaseDto>()
                .ReverseMap();
        }

        private IMappingExpression<TDto, WebBrowserSetting> CreateBaseDtoMap<TDto>()
            where TDto : WebBrowserSettingBaseDto
        {
            return CreateMap<TDto, WebBrowserSetting>()
                .ForMember(model => model.WebRTC, options => options.MapFrom(dto => dto.WebRTC))
                .ForMember(model => model.WebGL, options => options.MapFrom(dto => dto.WebGL))
                .ForMember(model => model.Tracking, options => options.MapFrom(dto => dto.Tracking))
                .ForMember(model => model.Flash, options => options.MapFrom(dto => dto.Flash))
                .ForMember(model => model.Canvas, options => options.MapFrom(dto => dto.Canvas))
                .ForMember(model => model.UserAgentId, options => options.MapFrom(dto => dto.UserAgentId))
                ;
        }
    }
}
