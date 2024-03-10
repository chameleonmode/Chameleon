using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.App.Settings;

public interface IUserProxySettingsViewModel
    : ISingletonDependency
{
    int FolderId { get; set; }
}
