using Chameleon.Interfaces.Ioc;
using Prism.Services.Dialogs;

namespace Chameleon.Interfaces.DialogWindows
{
    public interface IDialogWindowViewModel 
        : ITransientDependency
    {
        string Title { get; set; }
    }
}
