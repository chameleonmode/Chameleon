using Prism.Services.Dialogs;
using System;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Infrastructure.Dialogs
{
    using IDialogService = Interfaces.Dialogs.IDialogService;

    /// <summary>
    /// Wrapper on IDialogService to simplify usage and strong type naming
    /// </summary>
    public class DialogManager : IDialogManager
    {
        private readonly IDialogService _dialogService;
        public DialogManager(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public IDialog Create(Type dialogType, Action<IDialogResult> callback = null, IDialogParameters parameters = null)
        {
            if (callback == null)
            {
                callback = _ => { };
            }

            return _dialogService.Create(
                dialogType.GetDependencyName(),
                parameters, callback);
        }

        public void ShowModal(Type dialogType, Action<IDialogResult> callback = null, IDialogParameters parameters = null)
        {
            if (callback == null)
            {
                callback = _ => { };
            }

            _dialogService.ShowDialog(
                dialogType.GetDependencyName(), 
                parameters, callback);
        }
        
        public void Show(Type dialogType, Action<IDialogResult> callback = null, IDialogParameters parameters = null)
        {
            if (callback == null)
            {
                callback = _ => { };
            }

            _dialogService.Show(
                dialogType.GetDependencyName(), 
                parameters, callback);
        }
    }
}
