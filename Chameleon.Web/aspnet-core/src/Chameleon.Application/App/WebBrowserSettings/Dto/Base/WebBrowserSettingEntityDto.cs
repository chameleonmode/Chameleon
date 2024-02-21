using Abp.Application.Services.Dto;

namespace Chameleon.App
{
    public class WebBrowserSettingEntityDto
        : WebBrowserSettingBaseDto
        , IEntityDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
