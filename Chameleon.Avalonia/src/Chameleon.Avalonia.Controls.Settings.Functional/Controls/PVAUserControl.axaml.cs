using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Controls;

namespace Chameleon.Avalonia.Controls.Settings.Functional;

public partial class PVAUserControl : UserControl
{
    public static readonly StyledProperty<string?> TitleProperty =
       AvaloniaProperty.Register<PVAUserControl, string?>(nameof(Title));

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly StyledProperty<object> TitleContentProperty =
        AvaloniaProperty.Register<PVAUserControl, object>(nameof(TitleContent));
    public object TitleContent
    {
        get => GetValue(TitleContentProperty);
        set => SetValue(TitleContentProperty, value);
    }

    public static readonly StyledProperty<object> HeaderContentContentProperty =
        AvaloniaProperty.Register<PVAUserControl, object>(nameof(HeaderContentContent));
    public object HeaderContentContent
    {
        get => GetValue(HeaderContentContentProperty);
        set => SetValue(HeaderContentContentProperty, value);
    }

    public static readonly StyledProperty<object> ContentContentProperty =
        AvaloniaProperty.Register<PVAUserControl, object>(nameof(ContentContent));
    public object ContentContent
    {
        get => GetValue(ContentContentProperty);
        set => SetValue(ContentContentProperty, value);
    }

    public PVAUserControl()
    {
        InitializeComponent();

        Title = "PVAUserControl";
    }
}