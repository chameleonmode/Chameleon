using Chameleon.Interfaces.App.UserSettings;
using Chameleon.Interfaces.Dialogs.ViewModels;
using Chameleon.Interfaces.Dialogs.Views;
using Chameleon.Interfaces.DialogWindows;
using Chameleon.Prism.Events;

namespace Chameleon.Av.Fluent.Dialogs.ViewModels;

public partial class BulkAddPagesPopupViewModel
       : DialogBase
       , IBulkAddPagesPopupViewModel
{
    [ObservableProperty]
    private string? _urls;

    public override async Task<IContentDialogResult> ShowAsync()
    {
        return await ContentDialogService.ShowContentDialogAsync(typeof(IBulkAddPagesPopupView));
    }
}
