using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Interfaces.Dialogs.Views;

namespace Chameleon.Av.Fluent.Dialogs;

public partial class DefaultContentDialogTitle : UserControl, IDefaultContentDialogTitle
{
    public DefaultContentDialogTitle()
    {
        InitializeComponent();
    }
}