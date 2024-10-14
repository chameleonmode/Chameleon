using AutoMapper;

using Chameleon.Domain.Entities;
using Chameleon.Infrastructure.Repositories;
using Chameleon.Interfaces.Bookmarks;
using Chameleon.Interfaces.Repository;
using Chameleon.lib.Common.Interfaces.Sys;


namespace Chameleon.Infrastructure.Bookmarks {
	public class BookmarkFileRepository
    : Repository<BookmarkFile,
            IBookmarkFile,
            int,
            BookmarkFileDto,
            CreateBookmarkFileDto,
            BookmarkFileDto,
            GetAllRequestDto
            >
        , IBookmarkFileRepository
    {
        public BookmarkFileRepository(
            IMapper mapper,
            IBookmarkFileApi apiClient,
            IEventAggregator eventAggregator
            ) : base(mapper, apiClient, eventAggregator)
        {
        }
    }
}
