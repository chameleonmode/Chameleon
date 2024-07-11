using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Controls;

namespace Chameleon.Av.Fluent.Dialogs;

public partial class TopMostSidePanelView : AutoViewModelLocatorControl, ITopMostSidePanelView
{
    public TopMostSidePanelView()
    {
        InitializeComponent();
    }
}