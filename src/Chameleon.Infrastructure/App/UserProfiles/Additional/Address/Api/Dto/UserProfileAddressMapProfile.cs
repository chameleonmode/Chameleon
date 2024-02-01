using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.UserProfiles.Additional;

namespace Chameleon.Infrastructure.UserProfiles.Api.Dto.Additional
{
    public class UserProfileAddressMapProfile: Profile
    {
        public UserProfileAddressMapProfile()
        {
            DtoMap();
            CreateDtoMap();
        }

        private void DtoMap()
        {
            var map = CreateMap<UserProfileAddressDto, UserProfileAddress>();
            map.ReverseMap();
        }

        private void CreateDtoMap()
        {
            CreateMap<IUserProfileAddress, CreateUserProfileAddressDto>();
        }
    }
}
