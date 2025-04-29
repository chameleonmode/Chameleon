using Avalonia.Controls;

namespace Chameleon.client.Features.Dashboard.Tags;

public partial class TagsView : UserControl {
    public TagsView() {
        InitializeComponent();
        DataContext = TagsViewModel.Instance;
    }
}