using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Settings
{
    public interface ISettingsView
        : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        void SetTabContent(SettingTabs tab);
    }
}