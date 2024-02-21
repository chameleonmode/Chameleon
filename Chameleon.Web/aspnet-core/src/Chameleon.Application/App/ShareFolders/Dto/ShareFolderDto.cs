using Abp.AutoMapper;
using Chameleon.App.Entities.ShareFolders;
using Chameleon.App.ShareFolders.Dto.Base;
using System.Collections.Generic;

namespace Chameleon.App.ShareFolders.Dto
{
    [AutoMap(typeof(UserFolder))]
    public class ShareFolderDto
        : ShareFolderEntityDto
    {
        public string FolderName { get; set; }
        public IList<ShareFolderPermissionDto> FolderPermissions { get; set; }
    }
}
