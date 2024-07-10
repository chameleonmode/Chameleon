using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace Chameleon.Av.Fluent.Common.Controls.Dialogs;

public class DialogInputHost : TemplatedControl
{
    /// <summary>
    /// Defines the <see cref="Text"/> property
    /// </summary>
    public static readonly DirectProperty<DialogInputHost, string> TextLabelProperty =
        AvaloniaProperty.RegisterDirect<DialogInputHost, string>(nameof(TextLabel),
            x => x.TextLabel, (x, v) => x.TextLabel = v,null, BindingMode.TwoWay);

    /// <summary>
    /// Gets or sets the Text associated with the TaskDialog control
    /// </summary>
    public string TextLabel
    {
        get => _textLabel;
        set => SetAndRaise(TextLabelProperty, ref _textLabel, value);
    }

    /// <summary>
    /// Defines the <see cref="Text"/> property
    /// </summary>
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<DialogInputHost, string?>(nameof(Text), null, false, BindingMode.TwoWay);

    /// <summary>
    /// Gets or sets the Text associated with the Dialog input control
    /// </summary>
    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    private string _textLabel;
}
