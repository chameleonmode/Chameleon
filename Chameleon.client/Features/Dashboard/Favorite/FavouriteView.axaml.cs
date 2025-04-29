using Avalonia.Controls;

namespace Chameleon.client.Features.Dashboard.Favorite;

public partial class FavouriteView : UserControl {
    public FavouriteView() {
        InitializeComponent();
        DataContext = FavouriteViewModel.Instance;
    }
}