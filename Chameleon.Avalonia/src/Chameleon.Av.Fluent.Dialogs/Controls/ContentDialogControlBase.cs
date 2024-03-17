using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Interfaces.Dialogs.Views;

namespace Chameleon.Av.Fluent.Dialogs.Controls;

public abstract class ContentDialogControlBase : UserControl,
    IContentDialogView
{
    public virtual string PrimaryButtonText => "OK";
    public virtual string SecondaryButtonText => string.Empty;
    public virtual string CloseButtonText => "Cancel";
    //public object? Title { get => Header; set => Header = value; }
    //public string PrimaryButtonText { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //public string SecondaryButtonText { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //public string CloseButtonText { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}
