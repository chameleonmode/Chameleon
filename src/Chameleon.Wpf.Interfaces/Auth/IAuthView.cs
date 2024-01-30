using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Views;

namespace Chameleon.Interfaces.Auth
{
    public interface IAuthView 
        : IViewControl
        , ISingletonDependency
    {
        IAuthViewModel ViewModel { get; }
    }
}
