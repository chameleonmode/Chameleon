using Abp.Application.Services;

namespace Chameleon.App
{
    public interface IOutReachLinkAppService
        : IAsyncCrudAppService<
            OutReachLinkDto,
            int,
            OutReachLinkGetAllRequestDto,
            CreateOutReachLinkDto,
            UpdateOutReachLinkDto
            >
    {
    }
}
