namespace Chameleon.Avalonia.Controls.Settings.Functional.ViewModels;

public partial class UserDefaultSettingsViewModel
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
        Title = "Default Home Pages";

        _userDefaultsSettingsService = userDefaultsSettingsService;
        _bulkAddPagesPopupViewModel = bulkAddPagesPopupView;

        EventAggregator
           .GetEvent<SelectedUserDefaultSettingEvent>()
           .Subscribe(_ => OnSelectedChanged());


    }
    public override async Task InitAsync(object? param)
    {
        await base.InitAsync(param);

        if (!Loaded)
            OnAuthenticated();
    }

    
    private const char BulkAddSeparator = ',';
    [RelayCommand]
    private async Task BulkAddPages()
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
            Save();
            ViewModels.Last().DefaultUrl = string.IsNullOrWhiteSpace(url) ? null : url.Trim();
        }
    }

    [RelayCommand]
    private void UnselectItems()
    {
        foreach (var setting in _mapping)
        {
            setting.IsChecked = false;
        }
    }

    [RelayCommand]
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

    [RelayCommand]
    private void CreateNewDefaultSettings()
    {
        EventAggregator
            .GetEvent<CreateUserDefaultSettingsEvent>()
            .Publish();
    }

    [RelayCommand]
    private void Save()
    {
        var viewModels = ViewModels.Where(m => m.HasChanged);

        foreach (var viewModel in viewModels)
        {
            viewModel.SaveUrlFromViewText();
        }

        //EventAggregator
        //    .GetEvent<CreateUserDefaultSettingsEvent>()
        //    .Publish();
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
