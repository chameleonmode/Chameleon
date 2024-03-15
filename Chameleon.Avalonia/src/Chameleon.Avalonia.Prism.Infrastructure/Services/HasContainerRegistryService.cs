using Chameleon.Interfaces.Ioc;
using DryIoc;
using Prism.DryIoc;
using Prism.Ioc;
using Chameleon.Core.Extensions;
using Chameleon.Common.Helpers;
using System.Xml.Linq;

namespace Chameleon.Avalonia.Prism.Infrastructure.Services;

public class HasContainerRegistryService : IHaveContainerRegistry
{
    private readonly IContainerRegistry _containerRegistry;

    public HasContainerRegistryService(IContainerRegistry containerRegistry)
    {
        _containerRegistry = containerRegistry;
        ContainerServiceHelper.Current.ContainerRegistry = this;
    }
 
    public void RegisterSingleton(Type from, Type to, string? name = null)
    {
        ContainerServiceHelper.Current.ContainerTypes.AddOrUpdate(from, new Tuple<Type, string>(to, name ?? to.Name));
        _containerRegistry.RegisterSingleton(from, to);
    }

    public void RegisterSingleton<F,T>(bool resolve = false, string? name = null) where T : F
    {
        RegisterSingleton(typeof(F), typeof(T), name);
        if(resolve)
            _containerRegistry.GetContainer().Resolve<T>();
    }

    public void Register(Type from, Type to)
    {
        ContainerServiceHelper.Current.ContainerTypes.AddOrUpdate(from, new Tuple<Type, string>(to,to.Name));
        _containerRegistry.Register(from, to);
    }

    public void RegisterScoped(Type from, Type to)
    {
        ContainerServiceHelper.Current.ContainerTypes.AddOrUpdate(from, new Tuple<Type, string>(to,to.Name));
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
