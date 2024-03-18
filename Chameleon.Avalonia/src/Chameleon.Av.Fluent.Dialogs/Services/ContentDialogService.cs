
using Chameleon.Common.Base;
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

    public async Task<IContentDialogResult> ShowContentDialogAsync(IDefaultContentDialogView contentDialog)
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

    public async Task<IContentDialogResult> ShowContentDialogAsync(ContentDialogButtons btns, object content, object? title = null, string primaryBtnTxt = "OK", string secondaryBtnTxt = "", string closebtnTxt = "")
    {
        return await ShowContentDialogAsync(new DefaultContentDialogView(btns, content, title, primaryBtnTxt, secondaryBtnTxt, closebtnTxt));
    }
}
