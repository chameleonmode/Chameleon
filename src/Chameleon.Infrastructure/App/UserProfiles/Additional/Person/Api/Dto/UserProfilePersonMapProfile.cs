using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.UserProfiles.Additional;

namespace Chameleon.Infrastructure.UserProfiles.Api.Dto.Additional
{
    public class UserProfilePersonMapProfile : Profile
    {
        public UserProfilePersonMapProfile()
        {
            DtoMap();
            CreateDtoMap();
        }

        private void DtoMap()
        {
            var map = CreateMap<UserProfilePersonDto, UserProfilePerson>();
            map.ReverseMap();
        }

        private void CreateDtoMap()
        {
            CreateMap<IUserProfilePerson, CreateUserProfilePersonDto>();
        }
    }
}
