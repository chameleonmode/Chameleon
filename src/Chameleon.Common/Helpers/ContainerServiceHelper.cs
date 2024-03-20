using Chameleon.Interfaces.Ioc;

namespace Chameleon.Common.Helpers;

public class ContainerServiceHelper
{
    private ContainerServiceHelper()
    {

    }
    public static ContainerServiceHelper Current { get; } = new ContainerServiceHelper();
    public Dictionary<Type, Tuple<Type,string>> ContainerTypes { get; } = [];
    public IHaveContainerProvider? ContainerProvider { get; set; }
    public IHaveContainerRegistry? ContainerRegistry { get; set; }

    public static T? Resolve<T>() 
    {
        return ContainerServiceHelper.Current.ContainerProvider.Resolve<T>();
    }

    public static object Resolve(Type t)
    {
        return ContainerServiceHelper.Current.ContainerProvider.Resolve(t);
    }
}