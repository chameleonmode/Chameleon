using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Startup
{
    public interface IApplicationStartup : ISingletonDependency
    {
        Task RunAsync();
        void Run();
    }
}
