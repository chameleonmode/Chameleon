using Chameleon.Infrastructure.Dto;

namespace Chameleon.Infrastructure.App.Users.PrimaryUser.Api.Dto
{
    public class PrimaryUserDto
        : IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
