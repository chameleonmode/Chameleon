using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Views;

namespace Chameleon.Interfaces.YouTube
{
    public interface IGoogleAuthenticationView
        : IViewControl
        , ITransientDependency
    {
        IGoogleAuthenticationViewModel ViewModel { get; }
    }
}
