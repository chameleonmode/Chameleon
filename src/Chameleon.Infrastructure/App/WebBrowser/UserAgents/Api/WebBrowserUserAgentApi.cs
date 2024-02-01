using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.WebBrowsers;
using Chameleon.Interfaces.Api;

namespace Chameleon.Infrastructure.UserProfiles.Api.Additional
{
    public class WebBrowserUserAgentApi
        : ApiLayer<WebBrowserUserAgentDto, int, CreateWebBrowserUserAgentDto, WebBrowserUserAgentDto>
        , IWebBrowserUserAgentApi
    {
        public WebBrowserUserAgentApi(IApiClient apiClient) 
            : base(apiClient, "WebBrowserUserAgent")
        { }
    }
}
