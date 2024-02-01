using Chameleon.Infrastructure.Api;
using Chameleon.Interfaces.Api;
using System;

namespace Chameleon.Infrastructure.Bookmarks
{
    public class BookmarkFileApi
        : ApiLayer<
            BookmarkFileDto
            , int
            , CreateBookmarkFileDto
            , BookmarkFileDto
            >
        , IBookmarkFileApi
    {
        public BookmarkFileApi(
            IApiClient apiClient
            ) : base(apiClient, "bookmarkFile")
        {
        }
    }
}

