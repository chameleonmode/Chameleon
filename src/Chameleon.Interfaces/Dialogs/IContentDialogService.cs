using Chameleon.Interfaces.Dialogs.Views;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Dialogs;

public interface IContentDialogService :
    ISingletonDependency
{
    Task<IContentDialogResult> ShowContentDialogAsync(Type contentDialog);
    Task<IContentDialogResult> ShowContentDialogAsync(IDefaultContentDialogView contentDialog);
    Task<IContentDialogResult> ShowContentDialogAsync(ContentDialogButtons btns, object content, object title);
    Task<IContentDialogResult> ShowContentDialogAsync(ContentDialogButtons btns, object content, 
        object? title = null, 
        string? primaryBtnTxt = null,
        string? secondaryBtnTxt = null,
        string? closebtnTxt = null);
    Task ShowContentDialogAsync(object content,  
        object? title = null,
        Action? action = null,
        IContentDialogResult onResult = IContentDialogResult.Primary, 
        ContentDialogButtons btns = ContentDialogButtons.YesNo,
        string? primaryBtnTxt = null,
        string? secondaryBtnTxt = null,
        string? closebtnTxt = null);
}