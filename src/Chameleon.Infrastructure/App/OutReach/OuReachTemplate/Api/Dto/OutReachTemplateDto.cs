using Chameleon.Infrastructure.Dto;

namespace Chameleon.Infrastructure.OutReach.Api.Dto
{
    public class OutReachTemplateDto
        : CreateOutReachTemplateDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
