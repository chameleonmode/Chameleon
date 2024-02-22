using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Modules
{
    public interface IModuleLoader<T> : ISingletonDependency
    {
        void LoadModules(T catalog);
    }
}
