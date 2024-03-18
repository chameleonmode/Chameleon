using Chameleon.Interfaces.Dialogs.ViewModels;
using Chameleon.Interfaces.Ioc;
using System;

namespace Chameleon.Interfaces.Dialogs.ViewModels
{
    public interface IBulkAddPagesPopupViewModel
    : IDialogViewModelBase,
    ISingletonDependency
    {
        string? Urls { set; get; }
    }
}
