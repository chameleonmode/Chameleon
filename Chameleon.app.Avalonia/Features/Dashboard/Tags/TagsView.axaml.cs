using Avalonia.Controls;

namespace Chameleon.app.Avalonia.Features.Dashboard.Tags;

public partial class TagsView : UserControl {
    public TagsView() {
        InitializeComponent();
        DataContext = TagsViewModel.Instance;
    }
}