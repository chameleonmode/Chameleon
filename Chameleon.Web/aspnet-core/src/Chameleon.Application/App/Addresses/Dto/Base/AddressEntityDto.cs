using Abp.Application.Services.Dto;

namespace Chameleon.App
{
    public class AddressEntityDto
        : AddressBaseDto
        , IEntityDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
