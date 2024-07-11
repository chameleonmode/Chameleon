using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Dialogs.Controls;
using Chameleon.Interfaces.Dialogs.ViewModels;
using Chameleon.Interfaces.Dialogs.Views;

namespace Chameleon.Av.Fluent.Dialogs;

public partial class BulkAddPagesPopupView : ContentDialogControlBase, IBulkAddPagesPopupView
{
    public override object? Title => ContainerServiceHelper.Current.ContainerProvider?.Resolve<IChameleonLogoDialogTitle>();
    public BulkAddPagesPopupView()
    {
        InitializeComponent();
        DataContext = ContainerServiceHelper.Resolve<IBulkAddPagesPopupViewModel>();
    }
}