using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Dialogs.ViewModels;

public interface IDefaultContentDialogContentViewModel : IViewAware,
    IDialogViewModelBase,
    IContentDialogAware,
    ISingletonDependency
{
}
