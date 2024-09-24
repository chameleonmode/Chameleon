using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.ExceptionOptions
{
    public interface IAppLoggerService
        : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        void Create(IAppLogger appLogger);
    }
}
