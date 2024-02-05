using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Settings
{
    public interface IApplicationSettingsService : ISingletonDependency
    {
        IApplicationSettings Get();
        Task Save();
    }
}