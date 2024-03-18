using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.App.Settings;

public interface IUserProxySettingsViewModel
    : ISubPageViewModel,
    ISingletonDependency
{
    int FolderId { get; set; }
}
