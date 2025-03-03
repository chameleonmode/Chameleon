using Avalonia.Controls;

namespace Chameleon.app.Avalonia.Features.Dashboard.Favourite;

[Chameleon.lib.Common.Attributes.ViewModel(typeof(FavouriteViewModel))]
public partial class FavouriteView : UserControl
{
    public FavouriteView()
    {
        InitializeComponent();
    }
}