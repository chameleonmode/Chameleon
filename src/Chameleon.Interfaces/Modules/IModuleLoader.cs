using Chameleon.Interfaces.Ioc;
using Prism.Modularity;

namespace Chameleon.Interfaces.Modules
{
    public interface IModuleLoader : ISingletonDependency
    {
        void LoadModules(IModuleCatalog catalog);
    }
}
