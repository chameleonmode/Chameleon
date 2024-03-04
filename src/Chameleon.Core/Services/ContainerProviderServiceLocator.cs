using Chameleon.Interfaces.Ioc;

namespace Chameleon.Core.Services;

//TODO: move to common
public class ContainerProviderServiceLocator
{
    private IHaveContainerProvider? containerProvider;

    private ContainerProviderServiceLocator()
    {

    }
    public static ContainerProviderServiceLocator Current { get; } = new ContainerProviderServiceLocator();
    public IHaveContainerProvider ContainerProvider
    {
        get => containerProvider ?? throw new ArgumentNullException();
        set => containerProvider ??= value;
    }
}