using Chameleon.Avalonia.Prism.Module.Base;

namespace Chameleon.Controls.ImportExport.Models
{
    public class ImportColumnItemViewModel : 
        ViewModelBase
        , IImportColumnItemViewModel
    {
        public ImportColumnItemViewModel(string value)
        {
            _value = value ?? string.Empty;
        }

        private string _value;
        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
    }
}
