using Avalonia;
using Avalonia.Controls;

namespace Chameleon.Av.Fluent.Common.Controls.Dialogs;

public class DialogInputHost : TextBox
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

    private string _textLabel;
}
