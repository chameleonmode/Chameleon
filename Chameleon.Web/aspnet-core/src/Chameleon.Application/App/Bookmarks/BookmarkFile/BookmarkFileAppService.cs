using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Chameleon.App.Entities;
using System.Threading.Tasks;

namespace Chameleon.App
{
    public class BookmarkFileAppService
         : AsyncCrudAppService<
            BookmarkFile,
            BookmarkFileDto,
            int,
            PagedAndSortedResultRequestDto,
            CreateBookmarkFileDto,
            UpdateBookmarkFileDto
            >
        , IBookmarkFileAppService
    {
        public BookmarkFileAppService(
          IRepository<BookmarkFile> repository)
            : base(repository)
        {
            LocalizationSourceName = ChameleonConsts.LocalizationSourceName;
        }
    }
}
