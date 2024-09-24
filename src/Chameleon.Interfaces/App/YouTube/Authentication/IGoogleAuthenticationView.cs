using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Views;

namespace Chameleon.Interfaces.YouTube
{
    public interface IGoogleAuthenticationView
        : IViewControl
        , Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
    {
        IGoogleAuthenticationViewModel ViewModel { get; }
    }
}
