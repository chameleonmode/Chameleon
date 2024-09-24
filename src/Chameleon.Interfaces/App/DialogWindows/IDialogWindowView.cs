using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Views;

namespace Chameleon.Interfaces.DialogWindows
{
    public interface IDialogWindowView
        : IViewControl
        , Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
    {
        object InnerContent { get; set; }
        //string Title { get; set; }
    }
}
