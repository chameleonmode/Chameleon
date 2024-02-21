using AutoMapper;
using Chameleon.App.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.App
{
    public class BookmarkMapProfile : AutoMapper.Profile
    {
        public BookmarkMapProfile()
        {
            CreateEntityDtoMap<BookmarkDto>()
                .ReverseMap()
                ;

            CreateBaseDtoMap<BookmarkBaseDto>()
                .ReverseMap();

            CreateBaseDtoMap<CreateBookmarkDto>();
            CreateEntityDtoMap<UpdateBookmarkDto>();
        }

        private IMappingExpression<TDto, Bookmark> CreateEntityDtoMap<TDto>()
            where TDto : BookmarkEntityDto
        {
            return CreateBaseDtoMap<TDto>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                ;
        }

        private IMappingExpression<TDto, Bookmark> CreateBaseDtoMap<TDto>()
            where TDto : BookmarkBaseDto
        {
            return CreateMap<TDto, Bookmark>()
                .ForMember(model => model.Url, options => options.MapFrom(dto => dto.Url))
                .ForMember(model => model.Name, options => options.MapFrom(dto => dto.Name))
                .ForMember(model => model.BookmarkType, options => options.MapFrom(dto => dto.BookmarkType))
                .ForMember(model => model.BookmarkFiles, options => options.MapFrom(dto => dto.BookmarkFiles))
                .ForMember(model => model.ProfileId, options => options.MapFrom(dto => dto.ProfileId))
                ;
        }
    }
}
