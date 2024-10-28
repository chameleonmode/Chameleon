using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace Chameleon.Av.Fluent.Common.Controls.Dialogs;

public class DialogInputHost : TemplatedControl {
	/// <summary>
	/// Defines the <see cref="TextLabel"/> property
	/// </summary>
	public static readonly StyledProperty<string?> TextLabelProperty = AvaloniaProperty.Register<DialogInputHost, string?>(nameof(TextLabel), null, false, BindingMode.TwoWay);
	public string? TextLabel {
		get => GetValue(TextLabelProperty);
		set => SetValue(TextLabelProperty, value);
	}

	/// <summary>
	/// Defines the <see cref="Text"/> property
	/// </summary>
	public static readonly StyledProperty<string?> TextProperty = AvaloniaProperty.Register<DialogInputHost, string?>(nameof(Text), null, false, BindingMode.TwoWay);
	public string? Text {
		get => GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}
}
