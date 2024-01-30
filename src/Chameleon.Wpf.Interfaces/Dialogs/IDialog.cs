

namespace Chameleon.Interfaces.Dialogs
{
    public interface IDialog : Prism.Dialogs.IDialogContainer
    {
        string Title { get; set; }
    }
}