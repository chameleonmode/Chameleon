using Chameleon.Interfaces.Ioc;
using DryIoc;
using Prism.DryIoc;
using Prism.Ioc;
using Chameleon.Core.Extensions;
using Chameleon.Common.Helpers;

namespace Chameleon.Avalonia.Prism.Infrastructure.Services;

public class HasContainerRegistryService : IHaveContainerRegistry
{
    private readonly IContainerRegistry _containerRegistry;

    public HasContainerRegistryService(IContainerRegistry containerRegistry)
    {
        _containerRegistry = containerRegistry;
        ContainerServiceHelper.Current.ContainerRegistry = this;
    }
 
    public void RegisterSingleton(Type from, Type to)
    {
        ContainerServiceHelper.Current.ContainerTypes.AddOrUpdate(from, to);
        _containerRegistry.RegisterSingleton(from, to);
    }

    public void RegisterSingleton<F,T>() where T : F
    {
        RegisterSingleton(typeof(F), typeof(T));
    }

    public void Register(Type from, Type to)
    {
        ContainerServiceHelper.Current.ContainerTypes.AddOrUpdate(from, to);
        _containerRegistry.Register(from, to);
    }

    public void RegisterScoped(Type from, Type to)
    {
        ContainerServiceHelper.Current.ContainerTypes.AddOrUpdate(from, to);
        _containerRegistry.RegisterScoped(from, to);
    }

    public bool IsRegistered(Type type)
    {
        return _containerRegistry.IsRegistered(type);
    }

    public bool IsRegistered(Type type, string name)
    {
        return _containerRegistry.IsRegistered(type, name);
    }


    public void ResolveAndRegister(Type interfaceType, Type objectType, string dependencyName)
    {
        var c = _containerRegistry.GetContainer();
       var o = c.Resolve(interfaceType);
       // c..Register(o, dependencyName, o);
    }

    public void RegisterInstance<TInterface>(TInterface instance)
    {
        _containerRegistry.GetContainer().RegisterInstance(instance);
    }

    public object Resolve(Type interfaceType)
    {
        return _containerRegistry.GetContainer().Resolve(interfaceType);
    }

    public void Register(Type objectType, Type interfaceType, string depName, Func<IHaveContainerRegistry, object> factoryMethoda)
    {
        Func<IContainerProvider, object> factoryMethod = c =>c.Resolve(interfaceType);
        _containerRegistry.Register(objectType, factoryMethod);
    }
}
