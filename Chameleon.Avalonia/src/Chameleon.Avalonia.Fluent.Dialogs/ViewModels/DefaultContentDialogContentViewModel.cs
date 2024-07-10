using Chameleon.Av.Fluent.Dialogs.Base;
using Chameleon.Interfaces.Dialogs.ViewModels;
using Chameleon.Interfaces.Dialogs.Views;

namespace Chameleon.Av.Fluent.Dialogs.ViewModels;

public partial class DefaultContentDialogContentViewModel : ContentDialogViewModelBase<IDefaultContentDialogContentView>,
    IDefaultContentDialogContentViewModel
{
    [ObservableProperty]
    private object _content = "some default content text";
}
