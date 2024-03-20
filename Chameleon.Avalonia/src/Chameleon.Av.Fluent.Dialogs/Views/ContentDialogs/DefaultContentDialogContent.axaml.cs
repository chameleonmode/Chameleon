using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Interfaces.Dialogs.Views;

namespace Chameleon.Av.Fluent.Dialogs;

public partial class DefaultContentDialogContentView : AutoViewModelLocatorControl,
    IViewModelAware,
    IDefaultContentDialogContentView
{
    public DefaultContentDialogContentView()
    {
        InitializeComponent();
    }

    public T GetDataContext<T>()
    {
        return (T)DataContext ?? ContainerServiceHelper.Resolve<T>();
    }
}