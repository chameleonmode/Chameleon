using Chameleon.Interfaces;
using Chameleon.Interfaces.Dialogs;

namespace Chameleon.CT.Common.Base;

public partial class ContentDialogViewModelBase : ObservableObjectBase,
    IContentDialogViewModel
{
    partial void OnDialogClosing(IContentDialogResult result);

    void IContentDialogViewModel.OnDialogClosing(IContentDialogResult result)
    {
        OnDialogClosing(result);
    }
}
