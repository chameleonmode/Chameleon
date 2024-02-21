using Abp.Application.Services.Dto;

namespace Chameleon.App
{
    public class BusinessEntityDto
        : BusinessBaseDto
        , IEntityDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
