using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.OutReach;

namespace Chameleon.Infrastructure.OutReach.Api.Dto
{
    public class UserProfileOutReachRssMapProfile : Profile
    {
        public UserProfileOutReachRssMapProfile()
        {
            DtoMap();
            CreateDtoMap();
        }

        private void DtoMap()
        {
            var map = CreateMap<UserProfileOutReachRssDto, IUserProfileOutReachRss>()
               .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
               .ForMember(model => model.RssName, options => options.MapFrom(dto => dto.RssName))
               .ForMember(model => model.RssLink, options => options.MapFrom(dto => dto.RssLink))
               .ForMember(model => model.ContactEmail, options => options.MapFrom(dto => dto.ContactEmail))
               .ForMember(model => model.ContactName, options => options.MapFrom(dto => dto.ContactName))
               .ForMember(model => model.Notes, options => options.MapFrom(dto => dto.Notes))
               .ForMember(model => model.Status, options => options.MapFrom(dto => dto.Status))
               .ForMember(model => model.ProfileId, options => options.MapFrom(dto => dto.ProfileId));

            map.ReverseMap();
        }

        private void CreateDtoMap()
        {
            CreateMap<IUserProfileOutReachRss, CreateUserProfileOutReachRssDto>()
               .ForMember(model => model.RssName, options => options.MapFrom(dto => dto.RssName))
               .ForMember(model => model.RssLink, options => options.MapFrom(dto => dto.RssLink))
               .ForMember(model => model.ContactEmail, options => options.MapFrom(dto => dto.ContactEmail))
               .ForMember(model => model.ContactName, options => options.MapFrom(dto => dto.ContactName))
               .ForMember(model => model.Notes, options => options.MapFrom(dto => dto.Notes))
               .ForMember(model => model.Status, options => options.MapFrom(dto => dto.Status))
               .ForMember(model => model.ProfileId, options => options.MapFrom(dto => dto.ProfileId));
        }
    }


    //public class UserProfileOutReachRssMapProfile : Profile
    //{
    //    public UserProfileOutReachRssMapProfile()
    //    {
    //        DtoMap();
    //        CreateDtoMap();
    //    }

    //    private void DtoMap()
    //    {
    //        var map = CreateMap<UserProfileOutReachRssDto, UserProfileOutReachRss>()
    //           .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
    //           .ForMember(model => model.RssName, options => options.MapFrom(dto => dto.RssName))
    //           .ForMember(model => model.RssLink, options => options.MapFrom(dto => dto.RssLink))
    //           .ForMember(model => model.ContactEmail, options => options.MapFrom(dto => dto.ContactEmail))
    //           .ForMember(model => model.ContactName, options => options.MapFrom(dto => dto.ContactName))
    //           .ForMember(model => model.Notes, options => options.MapFrom(dto => dto.Notes))
    //           .ForMember(model => model.Status, options => options.MapFrom(dto => dto.Status))
    //           .ForMember(model => model.ProfileId, options => options.MapFrom(dto => dto.ProfileId));

    //        map.ReverseMap();
    //    }

    //    private void CreateDtoMap()
    //    {
    //        CreateMap<IUserProfileOutReachRss, CreateUserProfileOutReachRssDto>()
    //           .ForMember(model => model.RssName, options => options.MapFrom(dto => dto.RssName))
    //           .ForMember(model => model.RssLink, options => options.MapFrom(dto => dto.RssLink))
    //           .ForMember(model => model.ContactEmail, options => options.MapFrom(dto => dto.ContactEmail))
    //           .ForMember(model => model.ContactName, options => options.MapFrom(dto => dto.ContactName))
    //           .ForMember(model => model.Notes, options => options.MapFrom(dto => dto.Notes))
    //           .ForMember(model => model.Status, options => options.MapFrom(dto => dto.Status))
    //           .ForMember(model => model.ProfileId, options => options.MapFrom(dto => dto.ProfileId));
    //    }
    //}
}
