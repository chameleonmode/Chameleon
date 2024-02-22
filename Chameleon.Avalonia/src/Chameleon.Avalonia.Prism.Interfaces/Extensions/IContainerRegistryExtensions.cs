using Chameleon.Interfaces.AutoMapper;
using Chameleon.Interfaces.Ioc;
using Prism.Ioc;
using System.Reflection;

namespace Chameleon.Avalonia.Prism.Interfaces.Extensions;

public static class IContainerRegistryExtensions
{
    public static void RegisterTypesFrom(this IContainerProvider self, Assembly assembly)
    {
        self.Resolve<IIocManager>().RegisterTypes(assembly);
    }

    public static void RegisterMapperFrom(this IContainerProvider self, Assembly assembly)
    {
        self.Resolve<IAutoMapper>().RegisterMapper(assembly);
    }
}
