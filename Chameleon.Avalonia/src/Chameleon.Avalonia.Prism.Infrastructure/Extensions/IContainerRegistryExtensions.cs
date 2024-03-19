using Chameleon.Infrastructure.Ioc;
using Chameleon.Interfaces.Ioc;
using Prism.Ioc;
using System.Reflection;
using Chameleon.Avalonia.Prism.Infrastructure.Services;
using Chameleon.Infrastructure;
using Chameleon.Interfaces.AutoMapper;

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

        containerRegistry.RegisterSingleton<IIocManager, IocManager>();
                                                           
        self.RegisterTypesFrom(executingAssemblyBase);
        self.RegisterTypesFrom(executingAssembly);

        self.RegisterMapperFrom(executingAssemblyBase);
        self.RegisterMapperFrom(executingAssembly);

        return self;
    }

    public static void RegisterTypesFrom(this IContainerProvider self, Assembly assembly)
    {
        self.Resolve<IIocManager>().RegisterTypes(assembly);
    }

    public static void RegisterMapperFrom(this IContainerProvider self, Assembly assembly)
    {
        self.Resolve<IAutoMapper>().RegisterMapper(assembly);
    }
}
