using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Settings
{
    public interface IUserDefaultSettingsService
         : ISingletonDependency
    {
        IUserDefaultSetting Get(int defaultUrlId);
        IUserDefaultSettings GetAll();
        IUserDefaultSetting Create();
        void Delete(IUserDefaultSetting userDefaultSettings);
        void Save(IUserDefaultSetting userDefaultSettings);
        Task<string> GetRandomUrlAsync();
    }
}
