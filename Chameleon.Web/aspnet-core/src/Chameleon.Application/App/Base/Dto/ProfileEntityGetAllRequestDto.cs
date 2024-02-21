using Abp.Application.Services.Dto;
using Chameleon.App.Entities;

namespace Chameleon.App
{
    public class ProfileEntityGetAllRequestDto
        : PagedAndSortedResultRequestDto
        , IMayHaveProfile
    {
        [Identity]
        public int? ProfileId { get; set; }
    }
}
