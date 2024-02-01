using AutoMapper;
using Chameleon.Interfaces.Bookmarks;

namespace Chameleon.Infrastructure.Bookmarks
{
    public class BookmarkFileMapProfile : Profile
    {
        public BookmarkFileMapProfile()
        {
            BookmarkFileDtoMap();
            CreateBookmarkFileDtoMap();
        }

        private void BookmarkFileDtoMap()
        {
            var map = CreateMap<BookmarkFileDto, IBookmarkFile>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                .ForMember(model => model.Url, options => options.MapFrom(dto => dto.Url))
                .ForMember(model => model.Name, options => options.MapFrom(dto => dto.Name))
                .ForMember(model => model.BookmarkId, options => options.MapFrom(dto => dto.BookmarkId))
                ;

            map.ForAllOtherMembers(options => options.Ignore());

            map.ReverseMap()
                .ForAllOtherMembers(options => options.Ignore());
        }

        private void CreateBookmarkFileDtoMap()
        {
            CreateMap<IBookmarkFile, CreateBookmarkFileDto>()
                .ForMember(model => model.Url, options => options.MapFrom(dto => dto.Url))
                .ForMember(model => model.Name, options => options.MapFrom(dto => dto.Name))
                .ForMember(model => model.BookmarkId, options => options.MapFrom(dto => dto.BookmarkId))
                .ForAllOtherMembers(options => options.Ignore());
        }
    }
}
