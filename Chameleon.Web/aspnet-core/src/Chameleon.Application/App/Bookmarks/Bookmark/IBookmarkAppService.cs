using Abp.Application.Services;

namespace Chameleon.App
{
    public interface IBookmarkAppService
        : IAsyncCrudAppService<
            BookmarkDto,
            int,
            BookmarkGetAllRequestDto,
            CreateBookmarkDto,
            UpdateBookmarkDto
            >
    {
    }
}
