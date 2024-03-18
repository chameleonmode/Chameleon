using Chameleon.Interfaces.Dialogs.Views;

namespace Chameleon.Interfaces.Dialogs;

public interface IContentDialogService
{
    Task<IContentDialogResult> ShowContentDialogAsync(Type contentDialog);
    Task<IContentDialogResult> ShowContentDialogAsync(IDefaultContentDialogView contentDialog);
    Task<IContentDialogResult> ShowContentDialogAsync(ContentDialogButtons btns, object content, object? title = null, string primaryBtnTxt = "OK", string secondaryBtnTxt = "", string closebtnTxt = "");
}