using Abp.Application.Services;
using Abp.Application.Services.Dto;

namespace Chameleon.App
{
    public interface IBookmarkFileAppService
        : IAsyncCrudAppService<
            BookmarkFileDto,
            int,
            PagedAndSortedResultRequestDto,
            CreateBookmarkFileDto,
            UpdateBookmarkFileDto
            >
    {
    }
}
