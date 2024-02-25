using Prism.Ioc;
using System.Reflection;
using Chameleon.Domain;
using Chameleon.Avalonia.Prism.Interfaces.Extensions;

namespace Chameleon.Avalonia.Prism.Domain.Extensions;

public static class IContainerRegistryExtensions
{
    public static IContainerProvider AddDomain(this IContainerProvider self)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        self.RegisterTypesFrom(executingAssembly);
        self.RegisterTypesFrom(AssemblyResolver.GetAssembly());
        return self;
    }
}
