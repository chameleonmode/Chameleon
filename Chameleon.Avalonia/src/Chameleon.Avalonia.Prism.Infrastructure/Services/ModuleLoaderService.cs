using Chameleon.Infrastructure.Modules;
using Chameleon.Avalonia.Prism.Infrastructure.Extensions;
using Chameleon.Interfaces.Modules;
using Prism.Ioc;
using Prism.Modularity;
using System.Reflection;

namespace Chameleon.Avalonia.Prism.Infrastructure.Services;

public class ModuleLoaderService : ModuleLoader, IModuleLoader<IModuleCatalog>
{
    private readonly IContainerProvider _containerProvider;
    public ModuleLoaderService(IContainerProvider containerProvider)
    {
        _containerProvider = containerProvider;
    }

    public void LoadModules(IModuleCatalog catalog)
    {
        LoadModules();
        foreach (var module in GetModules())
        {
            catalog.AddModule(module);
        }
    }

    private IList<ModuleInfo> GetModules()
    {
        return Assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IModule).IsAssignableFrom(type))
            .Where(type => !type.IsAbstract)
            .Select(type => new ModuleInfo(type))
            .ToList();
    }

    public override void RegisterTypes(IList<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
        {
            _containerProvider.RegisterTypesFrom(assembly);
            _containerProvider.RegisterMapperFrom(assembly);
        }
    }
}
