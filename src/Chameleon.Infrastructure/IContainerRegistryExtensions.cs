using Chameleon.Infrastructure.Ioc;
using Chameleon.Interfaces.AutoMapper;
using Chameleon.Interfaces.Ioc;
using Prism.Ioc;
using System.Reflection;

namespace Chameleon.Infrastructure
{
    public static class IContainerRegistryExtensions
    {
        public static IContainerProvider AddInfrastructure(
            this IContainerProvider self,
            IContainerRegistry containerRegistry
            )
        {
            var executingAssembly = Assembly.GetExecutingAssembly();

            containerRegistry.RegisterSingleton<IIocManager, IocManager>();
            self.RegisterTypesFrom(executingAssembly);
            self.RegisterMapperFrom(executingAssembly);

            return self;
        }
    }
}
