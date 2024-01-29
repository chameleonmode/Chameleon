using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Settings
{
    public interface ISettingsView
        : ISingletonDependency
    {
        void SetTabContent(SettingTabs tab);
    }
}