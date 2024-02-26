using Chameleon.Avalonia.Prism.Interfaces.Dialogs;
using Chameleon.Avalonia.Prism.Interfaces.MessageBox;
using Chameleon.Avalonia.Prism.Module.MessageBox.ViewModels;
using Chameleon.Core.Extensions;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.MessageBox;
using Chameleon.Prism.Events;
using Prism.Services.Dialogs;
using System;
using System.Windows;

namespace Chameleon.Avalonia.Prism.Module.MessageBox.Services
{
    public class MessageBoxService : IMessageBoxService
    {
        private readonly IPopupDialogWinowService _popupDialogService;

        public MessageBoxService(
            IPopupDialogWinowService popupDialogService)
        {
            _popupDialogService = popupDialogService;
        }

        public void ShowDialog(IMessageBoxOptions messageBoxOptions, Action<ButtonResult> callback)
        {
            if (messageBoxOptions.DefaultButton == ButtonResult.None)
            {
                messageBoxOptions.DefaultButton = GetDefaultButton(messageBoxOptions.Buttons);
            }

            var parameters = new DialogParameters
            {
                { nameof(MessageBoxViewModel.Title), messageBoxOptions.Title },
                { nameof(MessageBoxViewModel.MessageBoxText), messageBoxOptions.Text },
                { nameof(MessageBoxViewModel.MessageBoxButtons), messageBoxOptions.Buttons },
                { nameof(MessageBoxViewModel.MessageBoxIcon), messageBoxOptions.Icon },
                { nameof(MessageBoxViewModel.ContentButtons), messageBoxOptions.ContentButtons },
                { nameof(MessageBoxViewModel.DefaultButton), messageBoxOptions.DefaultButton }
            };

            ButtonResult res = ButtonResult.None;
            _popupDialogService.ShowDialog(messageBoxOptions.Owner, nameof(MessageBoxView), parameters, (r) => 
            {
                callback(r?.Result ?? ButtonResult.None);
            });
        }

        private ButtonResult GetDefaultButton(MessageBoxButton messageBoxButtons)
        {
            if (messageBoxButtons == MessageBoxButton.OK)
            {
                return ButtonResult.OK;
            }

            if (messageBoxButtons == MessageBoxButton.YesNo)
            {
                return ButtonResult.No;
            }

            return ButtonResult.Cancel;
        }
    }
}
