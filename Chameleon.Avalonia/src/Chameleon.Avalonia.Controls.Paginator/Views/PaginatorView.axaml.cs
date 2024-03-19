using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Avalonia.Controls.Paginator.ViewModels;
using Chameleon.Interfaces.Paginator;

namespace Chameleon.Avalonia.Controls.Paginator;

public partial class PaginatorView : UserControl,
    IPaginatorView
{
    public PaginatorView()
    {
        InitializeComponent();
    }
}