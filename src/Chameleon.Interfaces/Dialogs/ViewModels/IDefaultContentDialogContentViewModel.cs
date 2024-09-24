using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Dialogs.ViewModels;

public interface IDefaultContentDialogContentViewModel : IViewAware,
    IDialogViewModelBase,
    IContentDialogAware,
    Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
{
}
