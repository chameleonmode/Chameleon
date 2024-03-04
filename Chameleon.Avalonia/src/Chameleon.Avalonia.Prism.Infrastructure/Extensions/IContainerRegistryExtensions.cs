using Chameleon.Infrastructure.Ioc;
using Chameleon.Interfaces.Ioc;
using Prism.Ioc;
using System.Reflection;
using Chameleon.Avalonia.Prism.Interfaces.Extensions;
using Chameleon.Avalonia.Prism.Infrastructure.Services;
using Chameleon.Infrastructure;
using Chameleon.Common.Services;

namespace Chameleon.Avalonia.Prism.Infrastructure.Extensions;

public static class IContainerRegistryExtensions
{
    public static IContainerProvider AddInfrastructure(
        this IContainerProvider self,
        IContainerRegistry containerRegistry
        )
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        var executingAssemblyBase = AssemblyResolver.GetAssembly();

        containerRegistry.RegisterSingleton<IHaveContainerRegistry, HasContainerRegistryService>(); 

        containerRegistry.RegisterSingleton<IHaveContainerProvider, HasContainerProviderService>();
        ContainerProviderServiceLocator.Current.ContainerProvider = self.Resolve<IHaveContainerProvider>();

        containerRegistry.RegisterSingleton<IIocManager, IocManager>();
                                                           
        self.RegisterTypesFrom(executingAssemblyBase);
        self.RegisterTypesFrom(executingAssembly);

        self.RegisterMapperFrom(executingAssemblyBase);
        self.RegisterMapperFrom(executingAssembly);

        return self;
    }
}
