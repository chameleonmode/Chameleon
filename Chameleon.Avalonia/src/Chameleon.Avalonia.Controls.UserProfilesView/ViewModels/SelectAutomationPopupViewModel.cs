using Chameleon.Avalonia.Controls.Automation.ViewModels;
using Chameleon.Core.Collections;
using Chameleon.Core.Collections.Views;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.App.Automation.Services;
using Chameleon.Interfaces.App.Automation.ViewModels;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.WebBrowser;

namespace Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;

public partial class SelectAutomationPopupViewModel
    : ObservableObjectBase
    , ISelectAutomationPopupViewModel
{
    private ObservableCollection<IAutomationScriptDescription, IAutomationScriptViewModel> _mapping;
    private readonly IAutomationService _automationService;

    public SelectAutomationPopupViewModel(
        IAutomationService automationService
        )
    {
        _automationService = automationService;
    }

    private ObservableCollectionView<IAutomationScriptViewModel> _viewModels;
    public ObservableCollectionView<IAutomationScriptViewModel> ViewModels
    {
        get
        {
            if (_viewModels == null && _mapping != null)
            {
                _viewModels = new ObservableCollectionView<IAutomationScriptViewModel>(_mapping);
            }

            return _viewModels;
        }
    }

    private IList<IUserProfile> _userProfiles;
    public IList<IUserProfile> UserProfiles
    {
        get => _userProfiles;
        set
        {
            SetProperty(ref _userProfiles, value);
        }
    }

    private IAutomationScriptViewModel _selectedScriptDescription;
    public IAutomationScriptViewModel SelectedScriptDescription
    {
        get { return _selectedScriptDescription; }
        set
        {
            if (_selectedScriptDescription != value)
            {
                _selectedScriptDescription = value;
                OnPropertyChanged(nameof(SelectedScriptDescription));
                OnPropertyChanged(nameof(IsSelectedScript));
            }
        }
    }

    private SystemBrowserType _selectedBrowser;
    public SystemBrowserType SelectedBrowser
    {
        get => _selectedBrowser;
        set => SetProperty(ref _selectedBrowser, value);
    }

    public bool IsSelectedScript => SelectedScriptDescription != null;

    public override async Task InitAsync(object? param)
    {
        await base.InitAsync(param);

        if (!Loaded)
        {
            Initialize();
        }
    }

    private void Initialize()
    {
        var scripts = _automationService.GetAll();

        _mapping = new ObservableCollection<IAutomationScriptDescription,
            IAutomationScriptViewModel>(scripts, script => new AutomationScriptViewModel(script, _automationService));

        OnPropertyChanged(nameof(ViewModels));
    }

    public void OnDialogClosing(IContentDialogResult result)
    {
        if (result == IContentDialogResult.Primary)
        {
            var browser = SelectedBrowser;
            var profiles = UserProfiles;
            var selectedScript = SelectedScriptDescription;
        }
    }
}
