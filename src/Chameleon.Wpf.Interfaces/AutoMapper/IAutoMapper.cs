using Chameleon.Interfaces.Ioc;
using System.Reflection;

namespace Chameleon.Interfaces.AutoMapper
{
    public interface IAutoMapper : ISingletonDependency
    {
        void RegisterMapper(Assembly assembly);
    }
}
