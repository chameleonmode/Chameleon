
using System;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Dialogs
{
    /// <summary>
    /// Extending Prism.Services.Dialogs.IDialogService
    /// </summary>
    public interface IDialogService 
        : Prism.Services.IPageDialogService
        , ISingletonDependency
    {
        IDialog Create(string name, IDialogParameters parameters, Action<IDialogResult> callback);
    }
}
