using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Settings
{
    public interface IUserSettingsService
        : ISingletonDependency
    {
        IUserSetting Get();
        //IUserSetting Create();
        //void Delete(IUserSetting userSettings);
        void Save(IUserSetting userSettings);
    }
}
