using Chameleon.Common.Helpers;
using Chameleon.Interfaces.Ioc;
using Prism.Ioc;

namespace Chameleon.Avalonia.Prism.Infrastructure.Services;

public class HasContainerProviderService : IHaveContainerProvider
{
    private readonly IContainerProvider _containerProvider;
    public HasContainerProviderService(IContainerProvider containerProvider)
    {
        _containerProvider = containerProvider;
        ContainerServiceHelper.Current.ContainerProvider = this;
    }
    public object Resolve(Type type)
    {
        return _containerProvider.Resolve(type);
    }
    public T? Resolve<T>()
    {
        return _containerProvider.Resolve<T>() ?? default;
    }

    public T Resolve<T>(Type type) where T : class
    {
        return Resolve(type) as T ?? default;
    } 
}

