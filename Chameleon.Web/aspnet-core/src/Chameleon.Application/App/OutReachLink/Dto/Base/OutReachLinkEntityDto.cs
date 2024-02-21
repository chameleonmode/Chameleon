using Abp.Application.Services.Dto;

namespace Chameleon.App
{
    public class OutReachLinkEntityDto
        : OutReachLinkBaseDto
        , IEntityDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
