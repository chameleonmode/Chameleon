using Chameleon.Infrastructure.Api;
using Chameleon.Interfaces.Api;

namespace Chameleon.Infrastructure.OutReachLink
{
    public class ProfileOutReachLinkApi
          : ApiLayer<
            ProfileOutReachLinkDto
            , int
            , CreateProfileOutReachLinkDto
            , ProfileOutReachLinkDto
            >
        , IProfileOutReachLinkApi
    {
        public ProfileOutReachLinkApi(IApiClient apiClient)
           : base(apiClient, "outReachLink")
        { }
    }
}
