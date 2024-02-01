using Chameleon.Infrastructure.Api;
using Chameleon.Interfaces.Api;

namespace Chameleon.Infrastructure.ExceptionOptions
{
    public class AppLoggerApi
        : ApiLayer<AppLoggerDto
            , int
            , CreateAppLoggerDto
            , AppLoggerDto>
        , IAppLoggerApi
    {
        public AppLoggerApi(
           IApiClient apiClient
           ) : base(apiClient, "appLogger")
        {
        }
    }
}
