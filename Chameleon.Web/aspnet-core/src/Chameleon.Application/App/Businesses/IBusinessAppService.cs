using Abp.Application.Services;

namespace Chameleon.App
{
    public interface IBusinessAppService
        : IAsyncCrudAppService<
            BusinessDto,
            int,
            BusinessGetAllRequestDto,
            CreateBusinessDto,
            UpdateBusinessDto
            >
    {
    }
}
