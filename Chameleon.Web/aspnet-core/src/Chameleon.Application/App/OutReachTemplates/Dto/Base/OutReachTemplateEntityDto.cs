using Abp.Application.Services.Dto;

namespace Chameleon.App
{
    public class OutReachTemplateEntityDto
         : OutReachTemplateBaseDto
        , IEntityDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
