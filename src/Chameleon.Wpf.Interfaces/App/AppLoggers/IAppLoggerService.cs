using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.ExceptionOptions
{
    public interface IAppLoggerService
        : ISingletonDependency
    {
        void Create(IAppLogger appLogger);
    }
}
