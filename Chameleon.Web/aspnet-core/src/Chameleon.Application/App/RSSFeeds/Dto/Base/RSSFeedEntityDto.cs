using Abp.Application.Services.Dto;

namespace Chameleon.App
{
    public class RSSFeedEntityDto
        : RSSFeedBaseDto
        , IEntityDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
