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

public partial class SelectAutomationPopupViewModel(
    IAutomationService _automationService)
    : ContentDialogViewModelBase
    , ISelectAutomationPopupViewModel
{
    private ObservableCollection<IAutomationScriptDescription, IAutomationScriptViewModel> _mapping;
    private ObservableCollectionView<IAutomationScriptViewModel> _viewModels;

    [ObservableProperty]
    private IList<IUserProfile> _userProfiles;

    [ObservableProperty]
    private SystemBrowserType _selectedBrowser = SystemBrowserType.Brave;

    [ObservableProperty]
    private IAutomationScriptViewModel _selectedScriptDescription;

    public bool IsSelectedScript => SelectedScriptDescription != null;
    public ObservableCollectionView<IAutomationScriptViewModel> ViewModels => _viewModels ??= new ObservableCollectionView<IAutomationScriptViewModel>(_mapping);

    public override async Task InitAsync(object? param)
    {
        await base.InitAsync(param);

        if (!Loaded)
        {
            _mapping = new ObservableCollection<IAutomationScriptDescription, IAutomationScriptViewModel>(
                    await _automationService.GetAll(), 
                    script => new AutomationScriptViewModel(script));

            OnPropertyChanged(nameof(ViewModels));
        }
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

    partial void OnSelectedScriptDescriptionChanged(IAutomationScriptViewModel? oldValue, IAutomationScriptViewModel newValue)
    {
        OnPropertyChanged(nameof(IsSelectedScript));
    }

    public void OnDialogClosing(IContentDialogResult result)
    {
        if (result != IContentDialogResult.Primary
            || SelectedBrowser == SystemBrowserType.Unknown)
        {
            return;
        }

        IAutomationScriptDescription script = SelectedScriptDescription.ScriptDescription;
        // _ = _automationBrowserService.RunScript(script, SelectedBrowser, UserProfiles);
    }
}
