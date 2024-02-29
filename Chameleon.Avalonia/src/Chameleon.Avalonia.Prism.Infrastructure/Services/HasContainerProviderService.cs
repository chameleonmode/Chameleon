using Chameleon.Interfaces.Ioc;
using Prism.Ioc;

namespace Chameleon.Avalonia.Prism.Infrastructure.Services;

public class HasContainerProviderService : IHaveContainerProvider
{
    private readonly IContainerProvider _containerProvider;
    public HasContainerProviderService(IContainerProvider containerProvider)
    {
        _containerProvider = containerProvider;
    }
    public object Resolve(Type type)
    {
        return _containerProvider.Resolve(type);
    }
    public T Resolve<T>()
    {
        return _containerProvider.Resolve<T>() ?? throw new ArgumentNullException();
    }
}

