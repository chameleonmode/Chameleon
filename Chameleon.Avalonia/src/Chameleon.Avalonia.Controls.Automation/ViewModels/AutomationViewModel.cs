using Avalonia.Collections;
using Avalonia.Controls;
using Chameleon.Avalonia.Common.Helpers;
using Chameleon.Avalonia.Controls.Paginator.ViewModels;
using Chameleon.Common.Helpers;
using Chameleon.Core.Collections;
using Chameleon.Core.Collections.Views;
using Chameleon.Core.Extensions;
using Chameleon.Core.Util;
using Chameleon.CT.Common.Base;
using Chameleon.Domain.Entities.Automation;
using Chameleon.Interfaces;
using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.App.Automation.Services;
using Chameleon.Interfaces.App.Automation.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Configuration;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Chameleon.Avalonia.Controls.Automation.ViewModels
{
    public partial class AutomationViewModel(IAutomationService automationService)
       : PageViewModelBase
       , IAutomationViewModel
    {
        private const string _pageTitle = "Pre-installed automations";

        private ObservableCollection<IAutomationScriptDescription, AutomationScriptViewModel> _mapping;
        public AvaloniaList<AutomationScriptViewModel> UserScripts { get; } = [];


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

        public override async Task InitAsync(object? param)
        {
            await base.InitAsync(param);

            if (!Loaded)
            {
                Title = _pageTitle;
                await Initialize();
                await InitializeUserScripts();

                if (Directory.Exists(UserScriptsDirectory))
                {
                    using var watcher = new FileSystemWatcher(@"C:\path\to\folder");

                    watcher.NotifyFilter = NotifyFilters.Attributes
                                         | NotifyFilters.CreationTime
                                         | NotifyFilters.DirectoryName
                                         | NotifyFilters.FileName
                                         | NotifyFilters.LastAccess
                                         | NotifyFilters.LastWrite
                                         | NotifyFilters.Security
                                         | NotifyFilters.Size;

                    watcher.Changed += OnChanged;
                    watcher.Created += OnCreated;
                    watcher.Deleted += OnDeleted;
                    watcher.Renamed += OnRenamed;
                    watcher.Error += OnError;

                    watcher.Filter = "*.cs";
                    watcher.IncludeSubdirectories = false;
                    watcher.EnableRaisingEvents = true;
                }
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

        [RelayCommand]
        private async Task SelectUserScriptFolder()
        {
            var dialog = ApplicationHelper.GetMainWindow().StorageProvider;
            var selected = await dialog.OpenFolderPickerAsync(new() { AllowMultiple = false });


            ConfigHelper.UserScriptsDirectory = UserScriptsDirectory = selected?[0]?.Path.AbsolutePath;
            await InitializeUserScripts();
        }

        private async Task InitializeUserScripts()
        {
            UserScriptsDirectory = ConfigHelper.UserScriptsDirectory;
            UserScripts.Clear();
            foreach (IAutomationScriptDescription item in await automationService.GetAll(UserScriptsDirectory))
            {
                UserScripts.Add(new(item));
            }
        }

        private async void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            await InitializeUserScripts();
        }

        private async void OnCreated(object sender, FileSystemEventArgs e)
        {
            string value = $"Created: {e.FullPath}";
            await InitializeUserScripts();
        }

        private async void OnDeleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Deleted: {e.FullPath}");
            await InitializeUserScripts(); 
        }

        private async void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"Renamed:");
            Console.WriteLine($"    Old: {e.OldFullPath}");
            Console.WriteLine($"    New: {e.FullPath}");
            await InitializeUserScripts();
        }

        private static void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private static void PrintException(Exception? ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
        }
    }
}
