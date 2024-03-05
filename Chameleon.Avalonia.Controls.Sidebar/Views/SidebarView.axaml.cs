using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Avalonia.Controls.Sidebar.ViewModels;
using Chameleon.Core.Attributes;

namespace Chameleon.Avalonia.Controls.Sidebar;

[ViewModel(typeof(SidebarViewModel))]
public partial class SidebarView : UserControl
{
    public SidebarView()
    {
        InitializeComponent();
    }
}