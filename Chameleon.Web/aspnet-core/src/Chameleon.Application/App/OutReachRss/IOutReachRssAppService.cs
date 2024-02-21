using Abp.Application.Services;

namespace Chameleon.App
{
    public interface IOutReachRssAppService
         : IAsyncCrudAppService<
            OutReachRssDto,
            int,
            OutReachRssGetAllRequestDto,
            CreateOutReachRssDto,
            UpdateOutReachRssDto
            >
    {
    }
}
