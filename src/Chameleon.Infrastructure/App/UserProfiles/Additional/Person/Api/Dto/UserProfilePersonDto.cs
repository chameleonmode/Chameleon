using Chameleon.Infrastructure.Dto;
using System;

namespace Chameleon.Infrastructure.UserProfiles.Api.Dto.Additional
{

    public class UserProfilePersonDto
        : CreateUserProfilePersonDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
