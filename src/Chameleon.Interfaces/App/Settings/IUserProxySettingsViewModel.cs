using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.App.Settings;

public interface IUserProxySettingsViewModel
    : ISubPageViewModel,
    Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
{
    int FolderId { get; set; }
}
