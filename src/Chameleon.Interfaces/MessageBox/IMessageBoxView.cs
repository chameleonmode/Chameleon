using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Views;

namespace Chameleon.Interfaces.MessageBox
{
    public interface IMessageBoxView 
        : IViewControl
        , Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
    {
        IMessageBoxViewModel ViewModel { get; }
    }
}
