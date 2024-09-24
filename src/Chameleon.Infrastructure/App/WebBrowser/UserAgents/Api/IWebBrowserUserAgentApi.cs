using Chameleon.Infrastructure.Api;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Infrastructure.WebBrowsers
{
    public interface IWebBrowserUserAgentApi
        : IApiLayer<WebBrowserUserAgentDto, int, CreateWebBrowserUserAgentDto, WebBrowserUserAgentDto>
        , Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
    }
}
