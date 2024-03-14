using Chameleon.Interfaces.Ioc;

namespace Chameleon.Common.Helpers;

public class ContainerServiceHelper
{
    private ContainerServiceHelper()
    {

    }
    public static ContainerServiceHelper Current { get; } = new ContainerServiceHelper();
    public Dictionary<Type, Type> ContainerTypes { get; } = [];
    public IHaveContainerProvider? ContainerProvider { get; set; }
    public IHaveContainerRegistry? ContainerRegistry { get; set; }
}