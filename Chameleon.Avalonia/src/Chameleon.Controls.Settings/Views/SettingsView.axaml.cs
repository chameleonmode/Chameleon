using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Chameleon.Avalonia.Controls.Settings.ViewModels;
using Chameleon.Common.Helpers;
using Chameleon.Core.Attributes;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.Dashboard;
using Chameleon.Interfaces.Settings;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Controls.Experimental;
using FluentAvalonia.UI.Navigation;

namespace Chameleon.Avalonia.Controls.Settings;

[ViewModel(typeof(SettingsViewModel))]
public partial class SettingsView : UserControl
        , ISettingsView
{
    public SettingsView()
    {
        InitializeComponent();
        DataContext = ContainerServiceHelper.Resolve<ISettingsViewModel>();
    }

    private void Default_Tapped(object? sender, TappedEventArgs e)
    {
        if (e.Source is Visual v)
        {
           // var lbi = v.FindAncestorOfType<ListBoxItem>(true);
           // if (lbi != null && lbi.DataContext is SubPageViewModelBase fci)
           // {
           //     var item = lbi.GetVisualDescendants()
           //         .Where(x => x is Viewbox && x.Name == "IconHost")
           //         .FirstOrDefault();
           //     _animationPage = fci;
           //
           //    
           // }
        }
    }

    public void SetTabContent(SettingTabs tab)
    {
        throw new NotImplementedException();
    }
}