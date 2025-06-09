using Avalonia;
using Avalonia.Data;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;

namespace Chameleon.client.UI.Controls;
public class DialogInputHost : TemplatedControl {
	/// <summary>
	/// Defines the <see cref="TextLabel"/> property
	/// </summary>
	public static readonly StyledProperty<string?> TextLabelProperty =
	AvaloniaProperty.Register<DialogInputHost, string?>(nameof(TextLabel), null, false, BindingMode.TwoWay);
	public string? TextLabel {
		get => GetValue(TextLabelProperty);
		set => SetValue(TextLabelProperty, value);
	}

	/// <summary>
	/// Defines the <see cref="Text"/> property
	/// </summary>
	public static readonly StyledProperty<string?> TextProperty =
	AvaloniaProperty.Register<DialogInputHost, string?>(nameof(Text), null, false, BindingMode.TwoWay);
	public string? Text {
		get => GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}
}

[TemplatePart("DialogsHost", typeof(ItemsControl))]
public class ChameleonDialogControl : HeaderedContentControl {
	private ItemsControl? _dialogInputsHost;

	/// <summary>
	/// Defines the <see cref="DialogInputs"/> property
	/// </summary>
	public static readonly DirectProperty<ChameleonDialogControl, IList<DialogInputHost>> DialogInputsroperty =
	AvaloniaProperty.RegisterDirect<ChameleonDialogControl, IList<DialogInputHost>>(
			nameof(DialogInputs), x => x.DialogInputs, (x, v) => x.DialogInputs = v);
	public IList<DialogInputHost> DialogInputs {
		get => _dialogInputs;
		set => SetAndRaise(DialogInputsroperty, ref _dialogInputs, value);
	}

	public static readonly StyledProperty<string?> TitleDescriptionProperty =
	AvaloniaProperty.Register<ChameleonDialogControl, string?>(nameof(TitleDescription));
	public string? TitleDescription {
		get => GetValue(TitleDescriptionProperty);
		set => SetValue(TitleDescriptionProperty, value);
	}
	private IList<DialogInputHost> _dialogInputs;
	public ChameleonDialogControl() {
		_dialogInputs = [];
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
		base.OnApplyTemplate(e);
		_dialogInputsHost = e.NameScope.Get<ItemsControl>("DialogsHost");
	}

	protected override void OnLoaded(RoutedEventArgs e) {
		base.OnLoaded(e);

		List<Control> commands = new(_dialogInputs);
		if (_dialogInputsHost != null)
			_dialogInputsHost.ItemsSource = commands;
	}
}
