using Avalonia.Controls;

namespace Chameleon.client.Features.Shared.Tags;

public partial class TagsView : UserControl {
    public TagsView() {
        InitializeComponent();
        DataContext = TagsViewModel.Instance;
    }
}