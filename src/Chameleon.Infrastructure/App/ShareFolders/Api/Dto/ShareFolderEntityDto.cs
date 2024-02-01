using Chameleon.Infrastructure.Dto;

namespace Chameleon.Infrastructure.App.ShareFolders.Api.Dto
{
    public class ShareFolderEntityDto
        : ShareFolderBaseDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
