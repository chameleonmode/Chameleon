using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Settings
{
    public interface ISettingsViewModel
        : ISingletonDependency
    {
        int SelectedIndex { get; set; }
    }
}
