
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
        };

        var res = await dialog.ShowAsync();
        return (IContentDialogResult)res;
    }
}
