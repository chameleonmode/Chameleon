using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Startup
{
    public interface IApplicationStartup : ISingletonDependency
    {
        void Run();
    }
}
