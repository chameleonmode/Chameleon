using Chameleon.Avalonia.Controls.Paginator.ViewModels;
using Chameleon.Core.Collections;
using Chameleon.Core.Collections.Views;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.App.Automation.Services;
using Chameleon.Interfaces.App.Automation.ViewModels;
using Chameleon.Interfaces.App.Synchronization.Events;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Prism.Events;
using CommunityToolkit.Mvvm.Input;
using Prism.Commands;

namespace Chameleon.Avalonia.Controls.Automation.ViewModels
{
    public partial class AutomationViewModel
       : PageViewModelBase
       , IAutomationViewModel
    {
        private const string _pageTitle = "Pre-installed automations";

        private ObservableCollection<IAutomationScriptDescription, AutomationScriptViewModel> _mapping;

        private readonly IAutomationService _automationService;

        public AutomationViewModel(
            IAutomationService automationService
            )
        {
            Title = _pageTitle;

            _automationService = automationService;
        }

        private ObservableCollectionView<AutomationScriptViewModel> _viewModels;
        public ObservableCollectionView<AutomationScriptViewModel> ViewModels
        {
            get
            {
                if (_viewModels == null && _mapping != null)
                {
                    _viewModels = new ObservableCollectionView<AutomationScriptViewModel>(_mapping);

                    _mapping.CollectionChanged += OnViewModelChange;
                    InitPaginator();
                }

                return _viewModels;
            }
        }

        private PaginatorViewModel _paginatorViewModel;
        public PaginatorViewModel PaginatorViewModel
        {
            get => _paginatorViewModel;
            set
            {
                if (SetProperty(ref _paginatorViewModel, value))
                {
                    _paginatorViewModel.ChangePageIndex += OnChangePage;
                }
            }
        }

        private int _totalCount;
        public int TotalCount
        {
            get => _totalCount;
            set
            {
                SetProperty(ref _totalCount, value);
            }
        }

        public override async Task InitAsync(object? param)
        {
            await base.InitAsync(param);

            if (!Loaded)
            {
                OnAuthenticated();
            }
        }

        private void OnAuthenticated()
        {
            Initialize();
        }

        private void InitPaginator()
        {
            PaginatorViewModel = new PaginatorViewModel(ViewModels.Count);
            ViewModels.Offset = PaginatorViewModel.Skip;
            ViewModels.Limit = PaginatorViewModel.OnPageItems;
            TotalCount = PaginatorViewModel.TotalCount;
        }

        private void OnViewModelChange(object sender, EventArgs args)
        {
            var count = _viewModels.Items.Count;
            PaginatorViewModel.TotalCount = count;
            TotalCount = count;

            OnPropertyChanged(nameof(ViewModels));
        }

        private void OnChangePage(object sendner, EventArgs args)
        {
            ViewModels.Offset = PaginatorViewModel.Skip;
        }

        private void Initialize()
        {
            var scripts = _automationService.GetAll();

            _mapping = new ObservableCollection<IAutomationScriptDescription, 
                AutomationScriptViewModel>(scripts, script => new AutomationScriptViewModel(script, _automationService));

            OnPropertyChanged(nameof(ViewModels));
        }
    }
}
