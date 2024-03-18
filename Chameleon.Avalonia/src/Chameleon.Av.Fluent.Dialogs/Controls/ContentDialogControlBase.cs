using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Interfaces.Dialogs.Views;

namespace Chameleon.Av.Fluent.Dialogs.Controls;

public abstract class ContentDialogControlBase : UserControl,
    IContentDialogView
{
    public virtual object? Title => ContainerServiceHelper.Current.ContainerProvider?.Resolve<IDefaultContentDialogTitle>();
    public virtual string PrimaryButtonText => "OK";
    public virtual string SecondaryButtonText => string.Empty;
    public virtual string CloseButtonText => "Cancel";
    public virtual object? DialogContent => throw new NotImplementedException();

    public T GetDataContext<T>()
    {
        return (T)DataContext ?? ContainerServiceHelper.Current.ContainerProvider.Resolve<T>();
    }
}
