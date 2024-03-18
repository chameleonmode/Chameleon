using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Dialogs.ViewModels;

namespace Chameleon.CT.Common.Base;

public abstract partial class DialogBase :  ObservableObjectBase, IDialogViewModelBase
{
    [ObservableProperty]
    public string? header;

    [ObservableProperty]
    public string? subHeader;

    [ObservableProperty]
    public bool isInputEnabled = true;

    public abstract Task<IContentDialogResult> ShowAsync();
}
