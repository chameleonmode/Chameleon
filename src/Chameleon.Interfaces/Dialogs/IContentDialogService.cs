using Chameleon.Interfaces.Dialogs.ViewModels;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Dialogs;

public interface IContentDialogService :
    Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
{
    Task<IContentDialogResult> ShowAsync<TView,TViewModel>(Action<TViewModel> initialize) where TViewModel : class;
    Task<IContentDialogResult> ShowContentDialogAsync(Type contentDialog);
    Task<IContentDialogResult> ShowContentDialogAsync(IContentDialogAware contentDialog);  
    Task<IContentDialogResult> ShowContentDialogAsync(string title,
        string content, 
        ContentDialogButtons btns = ContentDialogButtons.YesNo,
        IFontIconInfo? fontIconInfo = null);
    Task<IContentDialogResult> ShowContentDialogAsync(ContentDialogButtons btns, object content, object title);
    Task<IContentDialogResult> ShowContentDialogAsync(object content, Action<IContentDialogResult> OnClosing, string title = "False", ContentDialogButtons btns = ContentDialogButtons.OKCancel);
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