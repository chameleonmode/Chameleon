using Chameleon.Infrastructure.Dto;

namespace Chameleon.Infrastructure.OutReachLink
{
    public class ProfileOutReachLinkDto
        : CreateProfileOutReachLinkDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
