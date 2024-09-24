using System.Reflection;

namespace Chameleon.Interfaces.Ioc
{
    public interface IIocManager : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        void RegisterTypes(Assembly assembly);
    }
}
