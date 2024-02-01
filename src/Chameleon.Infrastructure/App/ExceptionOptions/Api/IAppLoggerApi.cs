using Chameleon.Infrastructure.Api;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Infrastructure.ExceptionOptions
{
    public interface IAppLoggerApi
        : IApiLayer<AppLoggerDto
            , int
            , CreateAppLoggerDto
            , AppLoggerDto>
        , ISingletonDependency
    {
    }
}
