using Avalonia.Controls;

namespace Chameleon.app.Avalonia.Features.Dashboard.Tags;

[Chameleon.lib.Common.Attributes.ViewModel(typeof(TagsViewModel))]
public partial class TagsView : UserControl
{
    public TagsView()
    {
        InitializeComponent();
    }
}