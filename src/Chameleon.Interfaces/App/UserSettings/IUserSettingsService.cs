using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Settings
{
    public interface IUserSettingsService
        : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        IUserSetting Get();
        //IUserSetting Create();
        //void Delete(IUserSetting userSettings);
        void Save(IUserSetting userSettings);
    }
}
