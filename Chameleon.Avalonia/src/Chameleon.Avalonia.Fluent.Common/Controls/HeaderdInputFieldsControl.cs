using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.LogicalTree;
using FluentAvalonia.UI.Controls;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Chameleon.Av.Fluent.Common.Controls;

[TemplatePart("TextBoxesHost", typeof(ItemsControl))]
public class HeaderdInputFieldsControl : HeaderedContentControl
{
    public static readonly StyledProperty<object?> CommandParameterProperty = AvaloniaProperty.Register<HeaderdInputFieldsControl, object?>("CommandParameter");

    private IList<TextBox> _textboxes;
    private IList<Button> _buttons;
    private ICommand _command;

    private bool _commandCanExecute = true;

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
    //
    // Summary:
    //     Gets or sets a parameter to be passed to the Avalonia.Controls.Button.Command.
    public object? CommandParameter
    {
        get
        {
            return GetValue(CommandParameterProperty);
        }
        set
        {
            SetValue(CommandParameterProperty, value);
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == CommandParameterProperty)
        {
            CanExecuteChanged(Command, change.NewValue);
        }
    }

    //
    // Summary:
    //     Called when the System.Windows.Input.ICommand.CanExecuteChanged event fires.
    //
    //
    // Parameters:
    //   sender:
    //     The event sender.
    //
    //   e:
    //     The event args.
    private void CanExecuteChanged(object? sender, EventArgs e)
    {
        CanExecuteChanged(Command, CommandParameter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CanExecuteChanged(ICommand? command, object? parameter)
    {
        if (((ILogical)this).IsAttachedToLogicalTree)
        {
            bool flag = command?.CanExecute(parameter) ?? true;
            if (flag != _commandCanExecute)
            {
                _commandCanExecute = flag;
                UpdateIsEffectivelyEnabled();
            }
        }
    }
}
