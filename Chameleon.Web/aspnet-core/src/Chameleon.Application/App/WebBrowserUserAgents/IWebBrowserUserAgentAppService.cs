using Abp.Application.Services;

namespace Chameleon.App
{
    public interface IWebBrowserUserAgentAppService
        : IAsyncCrudAppService<
            WebBrowserUserAgentDto,
            int,
            WebBrowserUserAgentGetAllRequestDto,
            CreateWebBrowserUserAgentDto,
            UpdateWebBrowserUserAgentDto
            >
    {
        int GetDefaultUserAgentId();
        void SetDefaultUserAgentId(int id);
    }
}
