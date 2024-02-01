using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.Bookmarks;

namespace Chameleon.Infrastructure.Bookmarks
{
    public class ProfileBookmarkDtoMapProfile : Profile
    {
        public ProfileBookmarkDtoMapProfile()
        {
            DtoMap();
            CreateDtoMap();
        }

        private void DtoMap()
        {
            var map = CreateMap<ProfileBookmarkDto, ProfileBookmark>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                .ForMember(model => model.Url, options => options.MapFrom(dto => dto.Url))
                .ForMember(model => model.Name, options => options.MapFrom(dto => dto.Name))
                .ForMember(model => model.ProfileId, options => options.MapFrom(dto => dto.ProfileId))
                .ForMember(model => model.BookmarkFiles, options => options.MapFrom(dto => dto.BookmarkFiles))
                .ForMember(model => model.BookmarkType, options => options.MapFrom(dto => dto.BookmarkType))
                ;

            map.ForAllOtherMembers(options => options.Ignore());

            map.ReverseMap()
                .ForAllOtherMembers(options => options.Ignore());
        }

        private void CreateDtoMap()
        {
            CreateMap<IProfileBookmark, CreateProfileBookmarkDto>()
                .ForMember(model => model.Url, options => options.MapFrom(dto => dto.Url))
                .ForMember(model => model.Name, options => options.MapFrom(dto => dto.Name))
                .ForMember(model => model.ProfileId, options => options.MapFrom(dto => dto.ProfileId))
                .ForMember(model => model.BookmarkType, options => options.MapFrom(dto => dto.BookmarkType))
                .ForAllOtherMembers(options => options.Ignore());
        }
    }
}
