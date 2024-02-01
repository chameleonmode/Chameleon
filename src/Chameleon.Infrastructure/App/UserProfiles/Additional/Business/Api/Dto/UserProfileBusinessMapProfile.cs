using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.UserProfiles.Additional;

namespace Chameleon.Infrastructure.UserProfiles.Api.Dto.Additional
{
    public class UserProfileBusinessMapProfile : Profile
    {
        public UserProfileBusinessMapProfile()
        {
            DtoMap();
            CreateDtoMap();
        }

        private void DtoMap()
        {
            var map = CreateMap<UserProfileBusinessDto, UserProfileBusiness>();
            map.ReverseMap();
        }

        private void CreateDtoMap()
        {
            CreateMap<IUserProfileBusiness, CreateUserProfileBusinessDto>();
        }
    }
}
