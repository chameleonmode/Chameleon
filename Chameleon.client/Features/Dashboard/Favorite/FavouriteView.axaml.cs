using Chameleon.client.UI.Controls;

namespace Chameleon.client.Features.Dashboard.Favorite;

public partial class FavouriteView : AutoViewModelLocatorControl {
    public FavouriteView() {
        InitializeComponent();
    }
    protected override object? ViewModel => FavouriteViewModel.Instance;
}