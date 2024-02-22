using System;
using Chameleon.Interfaces.Ioc;
using Chameleon.Prism.Services.Dialogs;

namespace Chameleon.Interfaces.Dialogs
{
    /// <summary>
    /// Extending Prism.Services.Dialogs.IDialogService
    /// </summary>
    public interface IDialogService 
        : Prism.Services.Dialogs.IDialogService
        , ISingletonDependency
    {
        IDialog Create(string name, IDialogParameters parameters, Action<IDialogResult> callback);
    }
}
