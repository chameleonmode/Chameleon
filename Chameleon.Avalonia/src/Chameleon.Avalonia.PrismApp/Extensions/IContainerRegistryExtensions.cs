using Prism.Ioc;
using System.Reflection;
using Chameleon.Avalonia.Prism.Interfaces.Extensions;

namespace Chameleon.Avalonia.PrismApp.Extensions;

public static class IContainerRegistryExtensions
{
    public static IContainerProvider AddUi(this IContainerProvider self)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();

        self.RegisterTypesFrom(executingAssembly);

        return self;
    }
}
