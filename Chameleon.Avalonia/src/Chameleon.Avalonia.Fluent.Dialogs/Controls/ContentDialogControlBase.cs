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

    public ContentDialogButtons DialogButtons { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public Task<IContentDialogResult> ShowAsync()
    {
        throw new NotImplementedException();
    }

    object? IContentDialogAware.Title { get => Title; set => throw new NotImplementedException(); }
    object? IContentDialogAware.DialogContent { get => DialogContent; set => throw new NotImplementedException(); }
    public string? Glyph { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}
