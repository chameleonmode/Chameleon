using Abp.Application.Services.Dto;

namespace Chameleon.App.ShareFolders.Dto.Base
{
    public class ShareFolderEntityDto 
        : ShareFolderBaseDto
        , IEntityDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
