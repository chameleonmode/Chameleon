using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Core.Attributes;
using Chameleon.Interfaces.UpgradePlan;

namespace Chameleon.Avalonia.Controls.UpgradePlan;

[ViewModel(typeof(IUpgradePlanViewModel))]
public partial class UpgradePlanPopupView : UserControl,
        IUpgradePlanPopupView
{
    public UpgradePlanPopupView()
    {
        InitializeComponent();
    }
}