using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.Rss;

namespace Chameleon.Infrastructure.Rss
{
    public class UserProfileRssFeedMapProfile : Profile
    {
        public UserProfileRssFeedMapProfile()
        {
            DtoMap();
            CreateDtoMap();
        }

        private void DtoMap()
        {
            var map = CreateMap<UserProfileRssFeedDto, UserProfileRssFeed>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                .ForMember(model => model.Url, options => options.MapFrom(dto => dto.Url))
                .ForMember(model => model.ProfileId, options => options.MapFrom(dto => dto.ProfileId))
                ;

            map.ForAllOtherMembers(options => options.Ignore());

            map.ReverseMap()
                .ForAllOtherMembers(options => options.Ignore());
        }

        private void CreateDtoMap()
        {
            CreateMap<IUserProfileRssFeed, CreateUserProfileRssFeedDto>()
                .ForMember(model => model.Url, options => options.MapFrom(dto => dto.Url))
                .ForMember(model => model.ProfileId, options => options.MapFrom(dto => dto.ProfileId))
                .ForAllOtherMembers(options => options.Ignore());
        }
    }
}
