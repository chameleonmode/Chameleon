using AutoMapper;
using Chameleon.App.Entities;

namespace Chameleon.App
{
    public class BookmarkFileMapProfile : AutoMapper.Profile
    {
        public BookmarkFileMapProfile()
        {
            CreateEntityDtoMap<BookmarkFileDto>()
              .ReverseMap();

            CreateBaseDtoMap<BookmarkFileBaseDto>()
              .ReverseMap();

            CreateBaseDtoMap<CreateBookmarkFileDto>();
            CreateEntityDtoMap<UpdateBookmarkFileDto>();
        }

        private IMappingExpression<TDto, BookmarkFile> CreateEntityDtoMap<TDto>()
            where TDto : BookmarkFileEntityDto
        {
            return CreateBaseDtoMap<TDto>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id));
        }

        private IMappingExpression<TDto, BookmarkFile> CreateBaseDtoMap<TDto>()
            where TDto : BookmarkFileBaseDto
        {
            return CreateMap<TDto, BookmarkFile>()
                 .ForMember(model => model.Url, options => options.MapFrom(dto => dto.Url))
                .ForMember(model => model.Name, options => options.MapFrom(dto => dto.Name))
                .ForMember(model => model.BookmarkId, options => options.MapFrom(dto => dto.BookmarkId));
        }    
    }
}
