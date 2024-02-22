using Prism.Ioc;
using System.Reflection;
using Chameleon.Avalonia.Prism.Interfaces.Extensions;
using Chameleon.Application;

namespace Chameleon.Avalonia.Prism.Application.Extensions;

public static class IContainerRegistryExtensions
{
    public static IContainerProvider AddApplication(this IContainerProvider self)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        var executingAssemblyBase = AssemblyResolver.GetAssembly();
                                                           
        self.RegisterTypesFrom(executingAssemblyBase);
        self.RegisterTypesFrom(executingAssembly);
                                                             
        self.RegisterMapperFrom(executingAssemblyBase);
        self.RegisterMapperFrom(executingAssembly);

        return self;
    }
}
