using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Settings
{
    public interface IApplicationSettingsService : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        //IApplicationSettings Get();
        Task<IApplicationSettings> GetAsync();
        Task Save();
        Task Logout();
    }
}