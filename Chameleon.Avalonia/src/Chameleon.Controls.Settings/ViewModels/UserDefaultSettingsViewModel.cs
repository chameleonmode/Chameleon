using Chameleon.Avalonia.Prism.Module.Base;
using Chameleon.Core.Collections.Views;
using Chameleon.Core.Collections;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.DialogWindows;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.UserSettings;
using Prism.Commands;
using Prism.Services.Dialogs;
using Chameleon.Interfaces.Ioc;
using Chameleon.Prism.Events;
using Chameleon.Interfaces.App.Settings;
using Chameleon.Interfaces.Dialogs.Views;
using Chameleon.Interfaces.App.UserSettings;
using Chameleon.Interfaces.Dialogs.ViewModels;
using Chameleon.Interfaces.Dialogs;
using Chameleon.CT.Common.Base;
using Chameleon.Common.Icons;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels;

public class UserDefaultSettingsViewModel
       : SubPageViewModelBase
       , IUserDefaultSettingsViewModel
{
    private readonly IBulkAddPagesPopupViewModel _bulkAddPagesPopupViewModel;
    private readonly IUserDefaultSettingsService _userDefaultsSettingsService;
    private ObservableCollection<IUserDefaultSetting, UserDefaultSettingViewModel> _mapping;

    public UserDefaultSettingsViewModel(
        IUserDefaultSettingsService userDefaultsSettingsService,
        IBulkAddPagesPopupViewModel bulkAddPagesPopupView
        )
    {
        _userDefaultsSettingsService = userDefaultsSettingsService;
        _bulkAddPagesPopupViewModel = bulkAddPagesPopupView;

        EventAggregator
           .GetEvent<SelectedUserDefaultSettingEvent>()
           .Subscribe(_ => OnSelectedChanged());

        CreateCommand = new DelegateCommand(CreateNewDefaultSettings);
        RemoveSelectedItemsCommand = new DelegateCommand(RemoveSelectedItems);
        UnselectItemsCommand = new DelegateCommand(UnselectItems);
        BulkAddPagesCommand = new DelegateCommand(BulkAddPages);

        Title = "Default Home Pages";
    }
    public override Task InitAsync()
    {
        OnAuthenticated();
        return base.InitAsync();
    }

    private const string DialogTitle = "BULK ADD PAGES";
    private const char BulkAddSeparator = ',';
    public DelegateCommand BulkAddPagesCommand { get; }
    private async void BulkAddPages()
    {
        var result = await _bulkAddPagesPopupViewModel.ShowAsync();
        if (result == IContentDialogResult.Primary)
        {
            AddPages(_bulkAddPagesPopupViewModel.Urls);
        }
        else
        {
            _bulkAddPagesPopupViewModel.Urls = null;
        }
    }

    private void AddPages(string urls)
    {
        if (!string.IsNullOrEmpty(urls))
        {
            AddPages(urls.Split(BulkAddSeparator));
        }
        _bulkAddPagesPopupViewModel.Urls = null;
    }

    private void AddPages(string[] urls)
    {
        foreach (var url in urls)
        {
            CreateNewDefaultSettings();
            ViewModels.Last().DefaultUrl = string.IsNullOrWhiteSpace(url) ? null : url.Trim();
        }
    }

    public DelegateCommand UnselectItemsCommand { get; }
    private void UnselectItems()
    {
        foreach (var setting in _mapping)
        {
            setting.IsChecked = false;
        }
    }

    public DelegateCommand RemoveSelectedItemsCommand { get; }
    private void RemoveSelectedItems()
    {
        if (_selectedDefaultSetting == null || _selectedDefaultSetting.Count == 0)
        {
            return;
        }

        foreach (var setting in _selectedDefaultSetting)
        {
            setting.DeleteDefaultSettings();
        }
        OnSelectedChanged();
    }

    public DelegateCommand CreateCommand { get; }

    private void CreateNewDefaultSettings()
    {
        var viewModels = ViewModels.Where(m => m.HasChanged);
       
        foreach (var viewModel in viewModels)
        {
            viewModel.SaveUrlFromViewText();
        }

        EventAggregator
            .GetEvent<CreateUserDefaultSettingsEvent>()
            .Publish();
    }

    private ObservableCollectionView<UserDefaultSettingViewModel> _viewModels;
    public ObservableCollectionView<UserDefaultSettingViewModel> ViewModels
    {
        get
        {
            if (_viewModels == null && _mapping != null)
            {
                _viewModels = new ObservableCollectionView<UserDefaultSettingViewModel>(_mapping);
            }
            return _viewModels;
        }
    }
    public void OnAuthenticated()
    {
        var userSettings = _userDefaultsSettingsService.GetAll();

        _mapping = new ObservableCollection<IUserDefaultSetting, UserDefaultSettingViewModel>(
            userSettings, userSetting => new UserDefaultSettingViewModel(EventAggregator, userSetting, _userDefaultsSettingsService)
            );

        OnPropertyChanged(nameof(ViewModels));
    }

    private List<UserDefaultSettingViewModel> _selectedDefaultSetting;
    private void OnSelectedChanged()
    {
        _selectedDefaultSetting = _mapping
            .Where(setting => setting.IsChecked)
            .ToList();

        SelectedCount = _selectedDefaultSetting.Count;
    }

    private int _selectedCount;
    public int SelectedCount
    {
        get => _selectedCount;
        set
        {
            if (SetProperty(ref _selectedCount, value))
            {
                OnPropertyChanged(nameof(HasSelectedItems));
            }
        }
    }

    public bool HasSelectedItems => SelectedCount > 0;
}
