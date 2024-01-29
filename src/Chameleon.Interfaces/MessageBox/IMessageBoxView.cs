using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Views;

namespace Chameleon.Interfaces.MessageBox
{
    public interface IMessageBoxView 
        : IViewControl
        , ITransientDependency
    {
        IMessageBoxViewModel ViewModel { get; }
    }
}
