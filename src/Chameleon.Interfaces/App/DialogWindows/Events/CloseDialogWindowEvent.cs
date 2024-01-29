using Prism.Events;
using Prism.Services.Dialogs;

namespace Chameleon.Interfaces.DialogWindows
{
    public class CloseDialogWindowEvent
        : PubSubEvent<ButtonResult>
    {
    }
}
