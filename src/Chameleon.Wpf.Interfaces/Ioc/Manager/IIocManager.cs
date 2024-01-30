using System.Reflection;

namespace Chameleon.Interfaces.Ioc
{
    public interface IIocManager : ISingletonDependency
    {
        void RegisterTypes(Assembly assembly);
    }
}
