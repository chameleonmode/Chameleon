using Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;
using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Core.Attributes;
using Chameleon.Interfaces.App.Automation.Views;

namespace Chameleon.Avalonia.Controls.UserProfilesView;

[ViewModel(typeof(SelectAutomationPopupViewModel))]
public partial class SelectAutomationPopupView
    : ViewControlBase<SelectAutomationPopupViewModel>
    , ISelectAutomationPopupView
{
    public SelectAutomationPopupView()
    {
        InitializeComponent();
    }
}
