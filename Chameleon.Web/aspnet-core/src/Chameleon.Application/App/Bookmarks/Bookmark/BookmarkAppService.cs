using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Chameleon.App.Entities;
using System.Linq;

namespace Chameleon.App
{
    public class BookmarkAppService
        : AsyncCrudAppService<
            Bookmark,
            BookmarkDto,
            int,
            BookmarkGetAllRequestDto,
            CreateBookmarkDto,
            UpdateBookmarkDto
            >
        , IBookmarkAppService
    {
        public BookmarkAppService(
           IRepository<Bookmark> repository
           ) : base(repository)
        {
            LocalizationSourceName = ChameleonConsts.LocalizationSourceName;
        }

        protected override IQueryable<Bookmark> CreateFilteredQuery(BookmarkGetAllRequestDto input)
        {
            return Repository
                .GetAllIncluding(a => a.BookmarkFiles)
                .Where(a => a.BookmarkType == BookmarkType.GlobalFolder && a.CreatorUserId == AbpSession.UserId ||
                            (a.ProfileId == input.ProfileId && a.BookmarkType != BookmarkType.GlobalFolder));
        }

        protected override IQueryable<Bookmark> ApplySorting(IQueryable<Bookmark> query, BookmarkGetAllRequestDto input)
        {
            query = base.ApplySorting(query, input);
            if (input.Sorting.IsNullOrEmpty())
            {
                query = query.OrderBy(entity => entity.CreationTime);
            }
            return query;
        }
    }
}
