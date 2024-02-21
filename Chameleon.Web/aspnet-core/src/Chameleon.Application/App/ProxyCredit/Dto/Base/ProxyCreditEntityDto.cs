using Abp.Application.Services.Dto;

namespace Chameleon.App
{
    public class ProxyCreditEntityDto
        : ProxyCreditBaseDto
        , IEntityDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
