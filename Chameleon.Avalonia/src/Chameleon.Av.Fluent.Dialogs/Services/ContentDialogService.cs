using Chameleon.Interfaces.Dialogs.ViewModels;
using Chameleon.Interfaces.Dialogs.Views;
using FluentAvalonia.UI.Controls;

namespace Chameleon.Av.Fluent.Dialogs.Services;

public class ContentDialogService : IContentDialogService
{
    public async Task<IContentDialogResult> ShowContentDialogAsync(Type contentDialog)
    {
        var c = ContainerServiceHelper.Current.ContainerProvider?.Resolve<IContentDialogView>(contentDialog);
        var dialog = new ContentDialog()
        {
            Title = c.Title,
            Content = c,
            PrimaryButtonText = c.PrimaryButtonText,
            SecondaryButtonText = c.SecondaryButtonText,
            CloseButtonText = c.CloseButtonText,
            DefaultButton = ContentDialogButton.Primary,
        };

        var res = await dialog.ShowAsync();
        return (IContentDialogResult)res;
    }

    public async Task<IContentDialogResult> ShowContentDialogAsync(IContentDialogAware contentDialog)
    {
        var dialog = new ContentDialog()
        {
            Title = contentDialog.Title,
            Content = contentDialog.DialogContent,
            PrimaryButtonText = contentDialog.PrimaryButtonText,
            SecondaryButtonText = contentDialog.SecondaryButtonText,
            CloseButtonText = contentDialog.CloseButtonText,
            DefaultButton = ContentDialogButton.Primary,
        };

        var res = await dialog.ShowAsync();
        return (IContentDialogResult)res;
    }



    public async Task<IContentDialogResult> ShowContentDialogAsync(ContentDialogButtons btns, object content,
        object? title = null,
        string? primaryBtnTxt = null,
        string? secondaryBtnTxt = null,
        string? closebtnTxt = null)
    {
        return await ShowContentDialogAsync(new DefaultContentDialogView(btns, content, title, primaryBtnTxt, secondaryBtnTxt, closebtnTxt));
    }   
    public async Task<IContentDialogResult> ShowContentDialogAsync(ContentDialogButtons btns, object content, object title)
    {
        return await ShowContentDialogAsync(btns, content, title, 
            null, 
            null, 
            null);
    }
    public async Task ShowContentDialogAsync(object content,  
        object? title = null,
        Action? action = null,
        IContentDialogResult onResult = IContentDialogResult.Primary, 
        ContentDialogButtons btns = ContentDialogButtons.YesNo,
        string? primaryBtnTxt = null,
        string? secondaryBtnTxt = null,
        string? closebtnTxt = null)
    {
        if (await ShowContentDialogAsync(btns, content, title, primaryBtnTxt, secondaryBtnTxt, closebtnTxt) == onResult)
            action?.Invoke();
    }

    public async Task<IContentDialogResult> ShowContentDialogAsync(string title, string content, ContentDialogButtons btns = ContentDialogButtons.YesNo, IFontIconInfo? fontIconInfo = null)
    {
        var v = ContainerServiceHelper.Resolve<IDefaultContentDialogContentView>();
        var c = v.GetDataContext<IDefaultContentDialogContentViewModel>();
        c.Title = title;
        c.DialogContent = content;
        c.DialogButtons = btns;
        c.Glyph = fontIconInfo?.Glyph;
        return await ShowContentDialogAsync(c, v);
    }
    internal async Task<IContentDialogResult> ShowContentDialogAsync(IDefaultContentDialogContentViewModel contentDialog, IDefaultContentDialogContentView v)
    {
        var dialog = new ContentDialog()
        {
            Title = "False",
            Content = v,
            PrimaryButtonText = contentDialog.PrimaryButtonText,
            SecondaryButtonText = contentDialog.SecondaryButtonText,
            CloseButtonText = contentDialog.CloseButtonText,
            DefaultButton = ContentDialogButton.Primary,
        };

        var res = await dialog.ShowAsync();
        return (IContentDialogResult)res;
    }
}
