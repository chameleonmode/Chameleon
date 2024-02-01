using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Views;

namespace Chameleon.Interfaces.DialogWindows
{
    public interface IDialogWindowView
        : IViewControl
        , ITransientDependency
    {
        object InnerContent { get; set; }
        string Title { get; set; }
    }
}
