using Chameleon.Interfaces.Ioc;
using System.Reflection;

namespace Chameleon.Interfaces.AutoMapper
{
    public interface IAutoMapper : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        void RegisterMapper(Assembly assembly);
    }
}
