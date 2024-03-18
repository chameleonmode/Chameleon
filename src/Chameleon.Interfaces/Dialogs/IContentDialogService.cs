using Chameleon.Interfaces.Dialogs.Views;

namespace Chameleon.Interfaces.Dialogs;

public interface IContentDialogService
{
    Task<IContentDialogResult> ShowContentDialogAsync(Type contentDialog);
    Task<IContentDialogResult> ShowContentDialogAsync(IDefaultContentDialogView contentDialog);
}