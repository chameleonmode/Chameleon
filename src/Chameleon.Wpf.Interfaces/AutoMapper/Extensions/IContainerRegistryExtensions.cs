using Prism.Ioc;
using System.Reflection;

namespace Chameleon.Interfaces.AutoMapper
{
    public static class IContainerRegistryExtensions
    {
        public static void RegisterMapperFrom(this IContainerProvider self, Assembly assembly)
        {
            self.Resolve<IAutoMapper>()
                .RegisterMapper(assembly);
        }
    }
}
