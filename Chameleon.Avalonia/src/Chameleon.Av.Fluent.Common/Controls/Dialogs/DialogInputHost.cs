using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Chameleon.Av.Fluent.Common.Controls.Dialogs;

public class DialogInputHost : TemplatedControl
{
    /// <summary>
    /// Defines the <see cref="Text"/> property
    /// </summary>
    public static readonly DirectProperty<DialogInputHost, string> TextLabelProperty =
        AvaloniaProperty.RegisterDirect<DialogInputHost, string>(nameof(TextLabel),
            x => x.TextLabel, (x, v) => x.TextLabel = v);

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
    public static readonly DirectProperty<DialogInputHost, string> TextProperty =
        AvaloniaProperty.RegisterDirect<DialogInputHost, string>(nameof(Text),
            x => x.Text, (x, v) => x.Text = v);

    /// <summary>
    /// Gets or sets the Text associated with the TaskDialog control
    /// </summary>
    public string Text
    {
        get => _text;
        set => SetAndRaise(TextLabelProperty, ref _text, value);
    }

    private string _text;
    private string _textLabel;
}
