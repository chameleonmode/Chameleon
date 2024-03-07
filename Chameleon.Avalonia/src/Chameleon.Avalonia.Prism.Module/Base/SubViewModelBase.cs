using Prism.Commands;
using Prism.Regions;

namespace Chameleon.Avalonia.Prism.Module.Base;

public class SubViewModelBase : ViewModelBase
{
    public IRegionNavigationJournal? Journal { get; set; }
    public SubViewModelBase()
    {
        Title = "SubViewModelBase";
    }

    public DelegateCommand CmdNavigateBack => new DelegateCommand(() =>
    {
        Journal?.GoBack();
    });

    bool CanGoBack()
    {
        return Journal != null && Journal.CanGoBack;
    }

    /// <summary>Navigation completed successfully.</summary>
    /// <param name="navigationContext">Navigation context.</param>
    public override void OnNavigatedTo(NavigationContext navigationContext)
    {
        // Used to "Go Back" to parent
        Journal = navigationContext.NavigationService.Journal;

        CmdNavigateBack.RaiseCanExecuteChanged();
        //CmdNavigateBack
    }
}
