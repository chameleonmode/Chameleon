using Abp.Application.Services.Dto;

namespace Chameleon.App
{
    public class OutReachRssEntityDto
        : OutReachRssBaseDto
        , IEntityDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
