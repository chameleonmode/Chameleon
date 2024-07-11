using System.Collections.ObjectModel;
using Chameleon.Avalonia.Common.Collections;
using Chameleon.Avalonia.Common.Helpers;
using Chameleon.Avalonia.Controls.Paginator.ViewModels;
using Chameleon.Common.Helpers;
using Chameleon.Core.Collections;
using Chameleon.Core.Collections.Views;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.App.Automation.Services;
using Chameleon.Interfaces.App.Automation.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Avalonia.Controls.Automation.ViewModels
{
    public partial class AutomationViewModel(IAutomationService automationService)
       : PageViewModelBase
       , IAutomationViewModel
    {
        private const string _pageTitle = "Pre-installed automations";

        private FileSystemWatcher? watcher;
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        private ObservableCollection<IAutomationScriptDescription, AutomationScriptViewModel> _mapping;
        
        public AvList<AutomationScriptViewModel> UserScripts { get; } = [];


        [ObservableProperty]
        private int _totalCount;

        [ObservableProperty]
        private string userScriptsDirectory = "";

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

        [RelayCommand]
        private async Task SelectUserScriptFolder()
        {
            var dialog = ApplicationHelper.GetMainWindow().StorageProvider;
            var selected = await dialog.OpenFolderPickerAsync(new() { AllowMultiple = false });
            if(selected == null || selected.Count == 0)
            {
                return;
            }

            ConfigHelper.UserScriptsDirectory = selected[0]?.Path?.AbsolutePath;
            await InitializeUserScripts();
        }

        public override async Task InitAsync(object? param)
        {
            await base.InitAsync(param);

            if (!Loaded)
            {
                Title = _pageTitle;
                await Initialize();
                await InitializeUserScripts();
            }
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

        private async Task Initialize()
        {
            var scripts = await automationService.GetAll();

            _mapping = new ObservableCollection<IAutomationScriptDescription,
                AutomationScriptViewModel>(scripts, script => new AutomationScriptViewModel(script));

            OnPropertyChanged(nameof(ViewModels));
        }

        private async Task InitializeUserScripts()
        {
            await semaphore.WaitAsync();
            try
            {
                UserScriptsDirectory = ConfigHelper.UserScriptsDirectory;
    
                if (!Directory.Exists(UserScriptsDirectory))
                    return;
    
                if (watcher == null)
                {
                    watcher = new(UserScriptsDirectory)
                    {
                        NotifyFilter = NotifyFilters.Attributes
                                     | NotifyFilters.CreationTime
                                     | NotifyFilters.DirectoryName
                                     | NotifyFilters.FileName
                                     | NotifyFilters.LastAccess
                                     | NotifyFilters.LastWrite
                                     | NotifyFilters.Security
                                     | NotifyFilters.Size,
                        Filter = "*.cs",
                        EnableRaisingEvents = true
                    };
                    
                    watcher.Changed += OnChanged;
                    watcher.Deleted += OnChanged;
                    watcher.Renamed += OnRenamed;
                    watcher.Created += OnChanged;
                } 
    
                //UserScripts.Clear();
                //await Task.Delay(50);
                UserScripts.UpdateMapped(await automationService.GetAll(UserScriptsDirectory), s => new(s), (x, y) => x.Filepath == y.FilePath);
                await Task.Delay(250);
            }
            finally
            {
                semaphore.Release();
            }
        }

        private async void OnChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"OnChanged: {e.ChangeType}");
            await InitializeUserScripts();
        }
        private async void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"Renamed:");
            Console.WriteLine($"    Old: {e.OldFullPath}");
            Console.WriteLine($"    New: {e.FullPath}");
            await InitializeUserScripts();
        }
    }
}
