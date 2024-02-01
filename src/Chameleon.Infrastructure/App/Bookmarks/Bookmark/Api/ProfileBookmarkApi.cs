using Chameleon.Infrastructure.Api;
using Chameleon.Interfaces.Api;

namespace Chameleon.Infrastructure.Bookmarks
{
    public class ProfileBookmarkApi
     : ApiLayer<
            ProfileBookmarkDto
            , int
            , CreateProfileBookmarkDto
            , ProfileBookmarkDto
            >
        , IProfileBookmarkApi
    {
        public ProfileBookmarkApi(IApiClient apiClient)
           : base(apiClient, "bookmark")
        { }
    }
}
