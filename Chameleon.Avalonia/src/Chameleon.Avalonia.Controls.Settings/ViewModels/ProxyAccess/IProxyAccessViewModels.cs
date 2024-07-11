using Chameleon.Interfaces.Ioc;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels.ProxyAccess;

public interface IProxyAccessViewModels
    : IList<ProxyAccessViewModel>
    , ITransientDependency
{
    void AddItems(int count);
}