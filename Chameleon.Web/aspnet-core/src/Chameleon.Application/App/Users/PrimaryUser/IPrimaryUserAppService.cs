using Abp.Application.Services;
using Chameleon.Users.Dto;

namespace Chameleon.App.Users.PrimaryUser
{
    public interface IPrimaryUserAppService : IAsyncCrudAppService<UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>
    {

    }
}
