using Chameleon.Controls.ImportExport.Models;
using Chameleon.Interfaces.Ioc;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Chameleon.Controls.ImportExport.ViewModels
{
    public class ImportColumnViewModels
        : ObservableCollection<IImportColumnViewModel>
        , IImportColumnViewModels
    {
        private readonly IHaveContainerProvider _containerProvider;
        public ImportColumnViewModels(IHaveContainerProvider containerProvider)
        {
            _containerProvider = containerProvider;
        }

        public IReadOnlyList<IImportColumnViewModel> Selected
            => this
            .Where(m => m.SelectedOption != null)
            .ToList()
            .AsReadOnly();

        public void ClearSelected()
        {
            foreach (var item in this)
            {
                item.SelectedOption = null;
            }
        }

        public bool HasSelected
            => this.Any(m => m.SelectedOption != null);

        public int ItemsCount => this
            .Select(m => m.Items.Count)
            .Max();

        protected override void ClearItems()
        {
            ClearSelected();
            base.ClearItems();
            TriggerPropertiesChange();
        }

        public IImportColumnViewModel GetOrCreateAt(int index)
        {
            while (index >= Count)
            {
                var item = _containerProvider.Resolve<IImportColumnViewModel>();
                item.PropertyChanged += Item_PropertyChanged;
                Add(item);
                return item;
            }
            return this[index];
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ImportColumnViewModel.SelectedOption))
            {
                TriggerPropertiesChange();
            }
        }

        private void TriggerPropertiesChange()
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Selected)));
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(HasSelected)));
        }
    }
}