using AutoMapper;
using Chameleon.App.Entities;

namespace Chameleon.App.Dto
{
    public class RSSFeedMapProfile : AutoMapper.Profile
    {
        public RSSFeedMapProfile()
        {
            CreateEntityDtoMap<RSSFeedDto>()
                .ReverseMap()
                ;

            CreateBaseDtoMap<RSSFeedBaseDto>()
                .ReverseMap();

            CreateBaseDtoMap<CreateRSSFeedDto>();
            CreateEntityDtoMap<UpdateRSSFeedDto>();
        }

        private IMappingExpression<TDto, RSSFeed> CreateEntityDtoMap<TDto>()
            where TDto : RSSFeedEntityDto
        {
            return CreateBaseDtoMap<TDto>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                ;
        }

        private IMappingExpression<TDto, RSSFeed> CreateBaseDtoMap<TDto>()
            where TDto : RSSFeedBaseDto
        {
            return CreateMap<TDto, RSSFeed>()
                .ForMember(model => model.Url, options => options.MapFrom(dto => dto.Url))
                .ForMember(model => model.ProfileId, options => options.MapFrom(dto => dto.ProfileId))
                ;
        }
    }
}
