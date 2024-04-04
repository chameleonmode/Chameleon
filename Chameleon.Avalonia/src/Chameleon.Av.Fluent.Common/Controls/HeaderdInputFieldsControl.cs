using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using FluentAvalonia.UI.Controls;
using System.Data.Common;
using System.Windows.Input;

namespace Chameleon.Av.Fluent.Common.Controls;

[TemplatePart("TextBoxesHost", typeof(ItemsPresenter))]
public class HeaderdInputFieldsControl : HeaderedContentControl
{
    public HeaderdInputFieldsControl()
    {
        _textboxes = new List<TextBox>();
        _buttons = new List<Button>();
    }
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<HeaderdInputFieldsControl, string?>(nameof(Title));

    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<HeaderdInputFieldsControl, string?>(nameof(Text),null,false,BindingMode.TwoWay);

    public static readonly StyledProperty<string?> TitleDescriptionProperty =
        AvaloniaProperty.Register<HeaderdInputFieldsControl, string?>(nameof(TitleDescription));

    /// <summary>
    /// Defines the <see cref="Command"/> property
    /// </summary>
    public static readonly DirectProperty<HeaderdInputFieldsControl, ICommand> CommandProperty =
        AvaloniaProperty.RegisterDirect<HeaderdInputFieldsControl, ICommand>(nameof(Command),
            x => x.Command, (x, v) => x.Command = v);


    public static readonly DirectProperty<HeaderdInputFieldsControl, IList<TextBox>> TextBoxesProperty =
        AvaloniaProperty.RegisterDirect<HeaderdInputFieldsControl, IList<TextBox>>(nameof(TextBoxes),
        x => x.TextBoxes, (x, v) => x.TextBoxes = v);

    public static readonly DirectProperty<HeaderdInputFieldsControl, IList<Button>> ButtonsProperty =
    AvaloniaProperty.RegisterDirect<HeaderdInputFieldsControl, IList<Button>>(nameof(Buttons),
    x => x.Buttons, (x, v) => x.Buttons = v);
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string? TitleDescription
    {
        get => GetValue(TitleDescriptionProperty);
        set => SetValue(TitleDescriptionProperty, value);
    }

    /// <summary>
    /// Gets or sets the command that is invoked when the button is clicked
    /// </summary>
    public ICommand Command
    {
        get => _command;
        set => SetAndRaise(CommandProperty, ref _command, value);
    }

    /// <summary>
    /// Gets the list of TextBox that display at the bottom of the TaskDialog
    /// </summary>
    public IList<TextBox> TextBoxes
    {
        get => _textboxes;
        set => SetAndRaise(TextBoxesProperty, ref _textboxes, value);
    }

    public IList<Button> Buttons
    {
        get => _buttons;
        set => SetAndRaise(ButtonsProperty, ref _buttons, value);
    }

    private IList<TextBox> _textboxes;
    private IList<Button> _buttons;
    private ICommand _command;
}
