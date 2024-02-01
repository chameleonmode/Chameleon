using AutoMapper;
using Chameleon.Interfaces.App.Users.PrimaryUser;

namespace Chameleon.Infrastructure.App.Users.PrimaryUser.Api.Dto
{
    public class PrimaryUserMapProfile : Profile
    {
        public PrimaryUserMapProfile()
        {
            var map = CreateMap<PrimaryUserDto, IPrimaryUser>();

            map.ReverseMap();
        }
    }
}
