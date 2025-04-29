using Avalonia.Controls;

namespace Chameleon.app.Avalonia.Features.Dashboard.Favourite;

public partial class FavouriteView : UserControl {
    public FavouriteView() {
        InitializeComponent();
        DataContext = FavouriteViewModel.Instance;
    }
}