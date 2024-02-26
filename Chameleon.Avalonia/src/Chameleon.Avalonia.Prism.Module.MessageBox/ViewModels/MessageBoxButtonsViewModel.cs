using Prism.Mvvm;
using Chameleon.Interfaces.MessageBox;

namespace Chameleon.Avalonia.Prism.Module.MessageBox.ViewModels
{
    public class MessageBoxContentButtonsViewModel 
        : BindableBase
        , IMessageBoxContentButtonsViewModel
    {
        private string _contentOkButton = "OK";
        public string ContentOkButton
        {
            get => _contentOkButton;
            set => SetProperty(ref _contentOkButton, value);
        }
        
        private string _contentNoButton = "No";
        public string ContentNoButton
        {
            get => _contentNoButton;
            set => SetProperty(ref _contentNoButton, value);
        }
        
        private string _contentCancelButton = "Cancel";
        public string ContentCancelButton
        {
            get => _contentCancelButton;
            set => SetProperty(ref _contentCancelButton, value);
        }

    }
}
