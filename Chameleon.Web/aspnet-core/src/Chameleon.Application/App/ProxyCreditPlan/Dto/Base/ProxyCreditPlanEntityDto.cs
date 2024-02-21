using Abp.Application.Services.Dto;

namespace Chameleon.App
{
    public class ProxyCreditPlanEntityDto
        : ProxyCreditPlanBaseDto
        , IEntityDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
