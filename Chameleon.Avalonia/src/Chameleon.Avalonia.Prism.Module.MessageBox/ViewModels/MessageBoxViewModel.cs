using Chameleon.Avalonia.Prism.Interfaces.MessageBox;
using Chameleon.Avalonia.Prism.Module.Base;
using Chameleon.Interfaces.MessageBox;
//using Chameleon.Module.Dialogs;
using Prism.Commands;
using Prism.Services.Dialogs;
using System.Drawing;
using System.Windows;

namespace Chameleon.Avalonia.Prism.Module.MessageBox.ViewModels
{
    public class MessageBoxViewModel : DialogViewModelBase, IMessageBoxViewModel
    {
        public MessageBoxViewModel()
        {
            OkCommand = new DelegateCommand(OkCloseDialog);
            NoCommand = new DelegateCommand(NoCloseDialog);
            CancelCommand = new DelegateCommand(CancelCloseDialog);
            EscCommand = new DelegateCommand(CloseDialog);
        }
        public override void OnDialogOpened(IDialogParameters parameters)
        {
            var title = parameters.GetValue<string>(nameof(Title));
            if (!string.IsNullOrEmpty(title))
            {
                Title = title;
            }

            var text = parameters.GetValue<string>(nameof(MessageBoxText));
            if (!string.IsNullOrEmpty(text))
            {
                MessageBoxText = text;
            }
            
            if (parameters.TryGetValue<MessageBoxButton>(nameof(MessageBoxButtons), out var buttons))
            {
                MessageBoxButtons = buttons;
            }
           

            if (parameters.TryGetValue<Icon>(nameof(MessageBoxIcon), out var icon))
            {
                MessageBoxIcon = icon;
            }

            if (parameters.TryGetValue<MessageBoxContentButtonsViewModel>(nameof(ContentButtons), out var contentButtons))
            {
                ContentButtons = contentButtons;
            }

            if (parameters.TryGetValue<ButtonResult>(nameof(DefaultButton), out var defaultButton))
            {
                DefaultButton = defaultButton;
            }
        }

        public void OkCloseDialog()
        {
            CloseDialog(ButtonResult.OK);
        }
        
        public void NoCloseDialog()
        {
            CloseDialog(ButtonResult.No);
        }
        
        public void CancelCloseDialog()
        {
            CloseDialog(ButtonResult.Cancel);
        }

        private string _title;// = Application.Current.MainWindow.Title;

        public override string Title
        {
            get => _title;
            set => SetProperty(ref _title, value); 
        }

        private string _text = string.Empty;

        public string MessageBoxText
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        private Icon _messageBoxIcon = SystemIcons.Information;

        public Icon MessageBoxIcon
        {
            get => _messageBoxIcon;
            set => SetProperty(ref _messageBoxIcon, value);
        }

        private MessageBoxButton _messageBoxButtons = MessageBoxButton.OK;

        public MessageBoxButton MessageBoxButtons
        {
            get => _messageBoxButtons;
            set => SetProperty(ref _messageBoxButtons, value);
        }

        private ButtonResult _defaultButton = ButtonResult.None;

        public ButtonResult DefaultButton
        {
            get => _defaultButton;
            set => SetProperty(ref _defaultButton, value);
        }

        private MessageBoxContentButtonsViewModel _contentButtons = new MessageBoxContentButtonsViewModel();

        public MessageBoxContentButtonsViewModel ContentButtons
        {
            get => _contentButtons;
            set => SetProperty(ref _contentButtons, value);
        }

        public DelegateCommand OkCommand { get; }
        public DelegateCommand NoCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public DelegateCommand EscCommand { get; }
    }
}
