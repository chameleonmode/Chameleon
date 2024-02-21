using Abp.Application.Services.Dto;

namespace Chameleon.App
{
    public class UpdateFolderDto 
        : FolderBaseDto
        , IEntityDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
