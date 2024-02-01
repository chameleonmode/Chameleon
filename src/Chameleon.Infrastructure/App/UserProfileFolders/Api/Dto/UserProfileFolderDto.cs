using Chameleon.Infrastructure.Dto;

namespace Chameleon.Infrastructure.UserProfileFolders
{
    public class UserProfileFolderDto
        : UserProfileFolderBaseDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
