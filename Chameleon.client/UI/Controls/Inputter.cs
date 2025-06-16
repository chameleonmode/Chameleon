using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.LogicalTree;

namespace Chameleon.client.UI.Controls;

public class Inputter : HeaderedContentControl {
	public static readonly StyledProperty<string?> TitleProperty =
		AvaloniaProperty.Register<Inputter, string?>(nameof(Title));
	public string? Title {
		get => GetValue(TitleProperty);
		set => SetValue(TitleProperty, value);
	}

	public static readonly StyledProperty<string?> TextProperty =
		AvaloniaProperty.Register<Inputter, string?>(nameof(Text), null, false, BindingMode.TwoWay, enableDataValidation: true);
	public string? Text {
		get => GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

	public static readonly StyledProperty<string?> DescriptionProperty =
		AvaloniaProperty.Register<Inputter, string?>(nameof(Description));
	public string? Description {
		get => GetValue(DescriptionProperty);
		set => SetValue(DescriptionProperty, value);
	}

	public static readonly StyledProperty<string?> WatermarkProperty =
		AvaloniaProperty.Register<Inputter, string?>(nameof(Watermark));
	public string? Watermark {
		get => GetValue(WatermarkProperty);
		set => SetValue(WatermarkProperty, value);
	}

	public static readonly DirectProperty<Inputter, IList<Inputter>> InputterzProperty =
		AvaloniaProperty.RegisterDirect<Inputter, IList<Inputter>>(nameof(Inputterz), x => x.Inputterz, (x, v) => x.Inputterz = v);
	public IList<Inputter> Inputterz {
		get => GetValue(InputterzProperty);
		set => SetValue(InputterzProperty, value);
	}

	override protected void OnApplyTemplate(TemplateAppliedEventArgs e) {
		base.OnApplyTemplate(e);
		// Additional template application logic can go here if needed

		var textblock = e.NameScope.Find<TextBlock>("TitleTextBlock");
		if (textblock != null && !string.IsNullOrEmpty(Description)) {
			textblock.Cursor = new Cursor(StandardCursorType.Help);
		}
	}
}

[TemplatePart("TextBoxesHost", typeof(ItemsControl))]
public class HeaderdInputFieldsControl : HeaderedContentControl {
	public static readonly StyledProperty<object?> CommandParameterProperty = AvaloniaProperty.Register<HeaderdInputFieldsControl, object?>("CommandParameter");

	private IList<TextBox> _textboxes;
	private IList<Button> _buttons;
	private ICommand? _command;

	private bool _commandCanExecute = true;

	public HeaderdInputFieldsControl() {
		_textboxes = [];
		_buttons = [];
	}
	public static readonly StyledProperty<string?> TitleProperty =
			AvaloniaProperty.Register<HeaderdInputFieldsControl, string?>(nameof(Title));
	public string? Title {
		get => GetValue(TitleProperty);
		set => SetValue(TitleProperty, value);
	}

	public static readonly StyledProperty<string?> TextProperty =
	AvaloniaProperty.Register<HeaderdInputFieldsControl, string?>(nameof(Text), null, false, BindingMode.TwoWay, enableDataValidation: true);
	public string? Text {
		get => GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

	public static readonly StyledProperty<string?> TitleDescriptionProperty =
			AvaloniaProperty.Register<HeaderdInputFieldsControl, string?>(nameof(TitleDescription));
	public string? TitleDescription {
		get => GetValue(TitleDescriptionProperty);
		set => SetValue(TitleDescriptionProperty, value);
	}

	/// <summary>
	/// Gets or sets the command that is invoked when the button is clicked
	/// </summary>
	public static readonly DirectProperty<HeaderdInputFieldsControl, ICommand?> CommandProperty =
			AvaloniaProperty.RegisterDirect<HeaderdInputFieldsControl, ICommand?>(nameof(Command),
					x => x.Command, (x, v) => x.Command = v);
	public ICommand? Command {
		get => _command;
		set => SetAndRaise(CommandProperty, ref _command, value);
	}

	public static readonly StyledProperty<string?> PlaceholderTextProperty =
	AvaloniaProperty.Register<HeaderdInputFieldsControl, string?>(nameof(PlaceholderText));
	public string? PlaceholderText {
		get => GetValue(PlaceholderTextProperty);
		set => SetValue(PlaceholderTextProperty, value);
	}

	/// <summary>
	/// Gets the list of TextBox that display at the bottom of the TaskDialog
	/// </summary>
	public static readonly DirectProperty<HeaderdInputFieldsControl, IList<TextBox>> TextBoxesProperty =
			AvaloniaProperty.RegisterDirect<HeaderdInputFieldsControl, IList<TextBox>>(nameof(TextBoxes),
			x => x.TextBoxes, (x, v) => x.TextBoxes = v);
	public IList<TextBox> TextBoxes {
		get => _textboxes;
		set => SetAndRaise(TextBoxesProperty, ref _textboxes, value);
	}

	public static readonly DirectProperty<HeaderdInputFieldsControl, IList<Button>> ButtonsProperty =
	AvaloniaProperty.RegisterDirect<HeaderdInputFieldsControl, IList<Button>>(nameof(Buttons),
	x => x.Buttons, (x, v) => x.Buttons = v);
	public IList<Button> Buttons {
		get => _buttons;
		set => SetAndRaise(ButtonsProperty, ref _buttons, value);
	}
	//
	// Summary:
	//     Gets or sets a parameter to be passed to the Avalonia.Controls.Button.Command.
	public object? CommandParameter {
		get {
			return GetValue(CommandParameterProperty);
		}
		set {
			SetValue(CommandParameterProperty, value);
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
		base.OnPropertyChanged(change);
		if (change.Property == CommandParameterProperty) {
			CanExecuteChanged(Command, change.NewValue);
		}
	}

	private bool isValidationInitialized = false;
	protected override void UpdateDataValidation(AvaloniaProperty property, BindingValueType state, Exception? error) {
		base.UpdateDataValidation(property, state, error);

		if (property == TextProperty && state != BindingValueType.DataValidationError) {
			DataValidationErrors.SetError(this, error);
		}

		if (property == TextProperty && state == BindingValueType.DataValidationError) {
			if (isValidationInitialized) {
				DataValidationErrors.SetError(this, error);
			} else {
				isValidationInitialized = true;
			}
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
	private void CanExecuteChanged(object? sender, EventArgs e) {
		CanExecuteChanged(Command, CommandParameter);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CanExecuteChanged(ICommand? command, object? parameter) {
		if (((ILogical)this).IsAttachedToLogicalTree) {
			var flag = command?.CanExecute(parameter) ?? true;
			if (flag != _commandCanExecute) {
				_commandCanExecute = flag;
				UpdateIsEffectivelyEnabled();
			}
		}
	}
}
