using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Chameleon.Controls.ImportExport.Models
{
    public interface IImportColumnViewModel : ITransientDependency, INotifyPropertyChanged
    {
        IImportColumnOption SelectedOption { get; set; }
        ObservableCollection<IImportColumnItemViewModel> Items { get; }
        void AddItem(IImportColumnItemViewModel cellViewModel);
        void MapTo(IUserProfile profile, int itemIndex);
    }    
}
