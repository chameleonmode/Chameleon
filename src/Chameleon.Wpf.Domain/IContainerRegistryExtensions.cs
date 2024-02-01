using Chameleon.Interfaces.Ioc;
using Prism.Ioc;
using System.Reflection;

namespace Chameleon.Domain
{
    public static class IContainerRegistryExtensions
    {
        public static IContainerProvider AddDomain(this IContainerProvider self)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            self.RegisterTypesFrom(executingAssembly);
            return self;
        }
    }
}
