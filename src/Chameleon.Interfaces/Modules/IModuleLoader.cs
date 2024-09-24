using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Modules
{
    public interface IModuleLoader<T> : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        void LoadModules(T catalog);
    }
}
