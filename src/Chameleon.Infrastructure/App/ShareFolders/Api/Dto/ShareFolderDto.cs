using Chameleon.Infrastructure.Dto;
using System.Collections.Generic;

namespace Chameleon.Infrastructure.App.ShareFolders.Api.Dto
{
    public class ShareFolderDto
        : ShareFolderEntityDto
        , IEntityDto<int>
    {
        public string FolderName { get; set; }
        public IList<ShareFolderPermissionDto> FolderPermissions { get; set; }
    }
}
