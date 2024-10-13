using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using Chameleon.Av.Fluent.Common.Controls;

namespace Chameleon.Avalonia.Controls.UserProfilesView;

public partial class TopMostSidePanelView : AutoViewModelInitControl {
    public TopMostSidePanelView()
    {
        InitializeComponent();
    }
	public static TopMostSidePanelView Instance { get; } = new TopMostSidePanelView();
}