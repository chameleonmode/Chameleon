using Chameleon.client.UI.Controls;

namespace Chameleon.client.Features.Dashboard.Tags;

public partial class TagsView : AutoViewModelLocatorControl {
    public TagsView() {
        InitializeComponent();
    }
    protected override object? ViewModel => TagsViewModel.Instance;
}