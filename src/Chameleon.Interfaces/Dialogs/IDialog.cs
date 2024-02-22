using Chameleon.Prism.Services.Dialogs;

namespace Chameleon.Interfaces.Dialogs
{
    public interface IDialog : IDialogWindow
    {
        string Title { get; set; }
    }
}