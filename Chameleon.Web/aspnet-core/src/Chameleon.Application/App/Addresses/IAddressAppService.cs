using Abp.Application.Services;

namespace Chameleon.App
{
    public interface IAddressAppService
        : IAsyncCrudAppService<
            AddressDto,
            int,
            AddressGetAllRequestDto,
            CreateAddressDto,
            UpdateAddressDto
            >
    {
    }
}
