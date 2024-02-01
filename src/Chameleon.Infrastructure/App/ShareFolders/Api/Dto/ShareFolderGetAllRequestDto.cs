using Chameleon.Interfaces.Repository;

namespace Chameleon.Infrastructure.App.ShareFolders.Api.Dto
{
    public class ShareFolderGetAllRequestDto
        : GetAllRequestDto
    {
        public long UserId { get; set; }
    }
}
