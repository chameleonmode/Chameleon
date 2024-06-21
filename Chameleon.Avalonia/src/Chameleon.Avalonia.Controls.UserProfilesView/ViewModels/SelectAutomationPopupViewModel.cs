using Chameleon.Avalonia.Controls.Automation.ViewModels;
using Chameleon.Core.Collections;
using Chameleon.Core.Collections.Views;
using Chameleon.CT.Common.Base;
using Chameleon.Domain.Entities.Automation;
using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.App.Automation.Services;
using Chameleon.Interfaces.App.Automation.ViewModels;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.WebBrowser;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;

public partial class SelectAutomationPopupViewModel(IAutomationService automationService)
    : ContentDialogViewModelBase
    , ISelectAutomationPopupViewModel
{
    private ObservableCollection<IAutomationScriptDescription, IAutomationScriptViewModel> _mapping;
    private ObservableCollectionView<IAutomationScriptViewModel> _viewModels;

    private readonly IAutomationService _automationService;
    private readonly IAutomationBrowserService _automationBrowserService;

    [ObservableProperty]
    private IList<IUserProfile> _userProfiles;

    [ObservableProperty]
    private SystemBrowserType _selectedBrowser = SystemBrowserType.Brave;

    [ObservableProperty]
    private IAutomationScriptViewModel _selectedScriptDescription;

    public SelectAutomationPopupViewModel(
        IAutomationService automationService,
        IAutomationBrowserService automationBrowserService
        )
    {
        _automationService = automationService;
        _automationBrowserService = automationBrowserService;
    }

    public bool IsSelectedScript => SelectedScriptDescription != null;
    public ObservableCollectionView<IAutomationScriptViewModel> ViewModels => _viewModels ??= new ObservableCollectionView<IAutomationScriptViewModel>(_mapping);

    public override async Task InitAsync(object? param)
    {
        await base.InitAsync(param);

        if (!Loaded)
        {
            _mapping = new ObservableCollection<IAutomationScriptDescription, IAutomationScriptViewModel>(
                    await Task.Run(automationService.GetAll), 
                    script => new AutomationScriptViewModel(script, automationService));

            OnPropertyChanged(nameof(ViewModels));
        }
    }

    public SystemBrowserType SelectedBrowser
    {
        get => _selectedBrowser;
        set => SetProperty(ref _selectedBrowser, value);
    }

    private ICommand _updateBrowserCommand;
    public ICommand UpdateBrowserCommand => _updateBrowserCommand ??= new RelayCommand<string>(UpdateBrowser);

    private void UpdateBrowser(string browser)
    {
        if (Enum.TryParse(browser, true, out SystemBrowserType browserEnum))
        {
            SelectedBrowser = browserEnum;
        }
    }

    public override async Task InitAsync(object? param)
    {
        await base.InitAsync(param);

        if (!Loaded)
        {
            _mapping = new ObservableCollection<IAutomationScriptDescription, IAutomationScriptViewModel>(
                    await Task.Run(automationService.GetAll), 
                    script => new AutomationScriptViewModel(script, automationService));

            OnPropertyChanged(nameof(ViewModels));
        }
    }

    partial void OnSelectedScriptDescriptionChanged(IAutomationScriptViewModel? oldValue, IAutomationScriptViewModel newValue)
    {
        OnPropertyChanged(nameof(IsSelectedScript));
    }


    [RelayCommand]
    private void UpdateBrowser(string browser)
    {
        if (Enum.TryParse(browser, true, out SystemBrowserType browserEnum))
        {
            SelectedBrowser = browserEnum;
        }

        if (result != IContentDialogResult.Primary
            || _selectedBrowser == SystemBrowserType.Unknown)
        {
            return;
        }
        
        IAutomationScriptDescription script = _selectedScriptDescription.ScriptDescription;
        _ = _automationBrowserService.RunScript(script, _selectedBrowser, _userProfiles);
    }
}
