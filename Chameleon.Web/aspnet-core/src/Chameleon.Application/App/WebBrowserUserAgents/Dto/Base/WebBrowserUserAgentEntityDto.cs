using Abp.Application.Services.Dto;

namespace Chameleon.App
{
    public class WebBrowserUserAgentEntityDto
        : WebBrowserUserAgentBaseDto
        , IEntityDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
