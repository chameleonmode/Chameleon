using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.UserProfiles.Additional;

namespace Chameleon.Infrastructure.UserProfiles.Api.Dto.Additional
{
    public class UserProfileLoginMapProfile : Profile
    {
        public UserProfileLoginMapProfile()
        {
            DtoMap();
            CreateDtoMap();
        }

        private void DtoMap()
        {
            var map = CreateMap<UserProfileLoginDto, UserProfileLogin>();
            map.ReverseMap();
        }

        private void CreateDtoMap()
        {
            CreateMap<IUserProfileLogin, CreateUserProfileLoginDto>();
        }
    }
}
