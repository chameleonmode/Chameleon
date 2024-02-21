using Abp.Application.Services;

namespace Chameleon.App
{
    public interface IRSSFeedAppService
        : IAsyncCrudAppService<
            RSSFeedDto,
            int,
            RSSFeedGetAllRequestDto,
            CreateRSSFeedDto,
            UpdateRSSFeedDto
            >
    {
    }
}
