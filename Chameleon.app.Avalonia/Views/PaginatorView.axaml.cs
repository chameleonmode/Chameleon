using Avalonia.Controls;
using Chameleon.Interfaces.Paginator;

namespace Chameleon.app.Avalonia.Views;

public partial class PaginatorView : UserControl,
    IPaginatorView
{
    public PaginatorView()
    {
        InitializeComponent();
    }
}