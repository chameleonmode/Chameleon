using Abp.Application.Services.Dto;
using Chameleon.App.Entities;
using System.ComponentModel.DataAnnotations;

namespace Chameleon.App.ShareFolders.Dto
{
    public class ShareFolderGetAllRequestDto 
        : PagedAndSortedResultRequestDto
        , IMustHaveUser
    {
        [Identity]
        [Required]
        public long UserId { get; set; }
    }
}
