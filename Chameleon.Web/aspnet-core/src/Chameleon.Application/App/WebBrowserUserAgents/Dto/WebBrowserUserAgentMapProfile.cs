using AutoMapper;
using Chameleon.App.Entities;

namespace Chameleon.App.Dto
{
    public class WebBrowserUserAgentMapProfile : AutoMapper.Profile
    {
        public WebBrowserUserAgentMapProfile()
        {
            CreateEntityDtoMap<WebBrowserUserAgentDto>()
                .ReverseMap()
                ;

            CreateBaseDtoMap<WebBrowserUserAgentBaseDto>()
                .ReverseMap();

            CreateBaseDtoMap<CreateWebBrowserUserAgentDto>();
            CreateEntityDtoMap<UpdateWebBrowserUserAgentDto>();
        }

        private IMappingExpression<TDto, WebBrowserUserAgent> CreateEntityDtoMap<TDto>()
            where TDto : WebBrowserUserAgentEntityDto
        {
            return CreateBaseDtoMap<TDto>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                ;
        }

        private IMappingExpression<TDto, WebBrowserUserAgent> CreateBaseDtoMap<TDto>()
            where TDto : WebBrowserUserAgentBaseDto
        {
            return CreateMap<TDto, WebBrowserUserAgent>()
                .ForMember(model => model.Name, options => options.MapFrom(dto => dto.Name))
                .ForMember(model => model.Value, options => options.MapFrom(dto => dto.Value))
                .ForMember(model => model.IsDefault, options => options.MapFrom(dto => dto.IsDefault))
                ;
        }
    }
}
