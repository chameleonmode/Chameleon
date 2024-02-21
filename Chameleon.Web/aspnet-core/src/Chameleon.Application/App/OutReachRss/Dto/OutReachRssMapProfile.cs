using AutoMapper;
using Chameleon.App.Entities;

namespace Chameleon.App
{
    public class OutReachRssMapProfile : AutoMapper.Profile
    {
        public OutReachRssMapProfile()
        {
            CreateEntityDtoMap<OutReachRssDto>()
                .ReverseMap();

            CreateBaseDtoMap<OutReachRssBaseDto>()
                .ReverseMap();

            CreateBaseDtoMap<CreateOutReachRssDto>();
            CreateEntityDtoMap<UpdateOutReachRssDto>();
        }

        private IMappingExpression<TDto, OutReachRss> CreateEntityDtoMap<TDto>()
            where TDto : OutReachRssEntityDto
        {
            return CreateBaseDtoMap<TDto>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id));
        }

        private IMappingExpression<TDto, OutReachRss> CreateBaseDtoMap<TDto>()
            where TDto : OutReachRssBaseDto
        {
            return CreateMap<TDto, OutReachRss>()
                .ForMember(model => model.ContactEmail, options => options.MapFrom(dto => dto.ContactEmail))
                .ForMember(model => model.ContactName, options => options.MapFrom(dto => dto.ContactName))
                .ForMember(model => model.Notes, options => options.MapFrom(dto => dto.Notes))
                .ForMember(model => model.RssLink, options => options.MapFrom(dto => dto.RssLink))
                .ForMember(model => model.RssName, options => options.MapFrom(dto => dto.RssName))
                .ForMember(model => model.Status, options => options.MapFrom(dto => dto.Status))
                .ForMember(model => model.ProfileId, options => options.MapFrom(dto => dto.ProfileId));
        }
    }
}
