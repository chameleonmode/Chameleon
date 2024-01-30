using Prism.Ioc;
using System.Reflection;

namespace Chameleon.Interfaces.Ioc
{
    public static class IContainerRegistryExtensions
    {
        public static void RegisterTypesFrom(this IContainerProvider self, Assembly assembly)
        {
            self.Resolve<IIocManager>().RegisterTypes(assembly);
        }
    }
}
