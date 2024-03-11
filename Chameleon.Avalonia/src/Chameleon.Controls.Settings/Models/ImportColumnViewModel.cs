using Chameleon.Interfaces.UserProfiles;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace Chameleon.Controls.ImportExport.Models
{
    public class ImportColumnViewModel 
        : BindableBase
        , IImportColumnViewModel
    {
        public ImportColumnViewModel(IImportColumnOptions importColumnOptions)
        {
            Options = importColumnOptions;
        }
        public IImportColumnOptions Options { get; }

        private IImportColumnOption _selectedOption;
        public IImportColumnOption SelectedOption
        {
            get => _selectedOption;
            set
            {
                if (value == UnselectedColumnOption.Instance)
                {
                    value = null;
                }

                if (SetProperty(ref _selectedOption, value))
                {
                    Options.Selected = value;
                }
            }
        }

        public ObservableCollection<IImportColumnItemViewModel> Items { get; }
            = new ObservableCollection<IImportColumnItemViewModel>();


        public void MapTo(IUserProfile profile, int itemIndex)
        {
            var itemValue = GetItemValueAt(itemIndex);
            SelectedOption.Map(profile, itemValue);
        }

        private string GetItemValueAt(int index)
        {
            if (index >= Items.Count)
            {
                return string.Empty;
            }
            return Items[index].Value;
        }

        public void AddItem(IImportColumnItemViewModel cellViewModel)
        {
            Items.Add(cellViewModel);
        }        
    }
}
