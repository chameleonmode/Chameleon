using Avalonia.Controls;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace Chameleon.client.UI.Controls;

public class ChameleonContentControl : HeaderedContentControl {
	private Button? expandOptionsButton;
	public ChameleonContentControl() {
		PseudoClasses.Add(":optionsfull");
		AttachedToVisualTree += (s, e) => {
			_ = ((VisualRoot as Window)?
			.GetObservable(TopLevel.ClientSizeProperty)
			.Subscribe(s => {
				IsOptionsExpanded = s.Width < 690; //ResponsiveConstants.MaxWindowWidth1080;
				_ = (expandOptionsButton?.SetValue(IsVisibleProperty, IsOptionsExpanded));
				UpdateIcon();
			}));
		};
	}

	public static readonly StyledProperty<string> IconShevronProperty =
	AvaloniaProperty.Register<ChameleonContentControl, string>(nameof(IconShevron));
	public string IconShevron {
		get => GetValue(IconShevronProperty);
		set => SetValue(IconShevronProperty, value);
	}

	public static readonly StyledProperty<string?> TitleProperty =
	AvaloniaProperty.Register<ChameleonContentControl, string?>(nameof(Title));
	public string? Title {
		get => GetValue(TitleProperty);
		set => SetValue(TitleProperty, value);
	}

	public static readonly StyledProperty<string?> TitleDescriptionProperty =
	AvaloniaProperty.Register<ChameleonContentControl, string?>(nameof(TitleDescription));
	public string? TitleDescription {
		get => GetValue(TitleDescriptionProperty);
		set => SetValue(TitleDescriptionProperty, value);
	}

	public static readonly StyledProperty<Control> OptionsProperty =
	AvaloniaProperty.Register<ChameleonContentControl, Control>(nameof(Options));
	public Control Options {
		get => GetValue(OptionsProperty);
		set => SetValue(OptionsProperty, value);
	}

	public static readonly StyledProperty<bool> IsOptionsExpandedProperty =
	AvaloniaProperty.Register<ChameleonContentControl, bool>(nameof(IsOptionsExpanded), true);
	public bool IsOptionsExpanded {
		get => GetValue(IsOptionsExpandedProperty);
		set => SetValue(IsOptionsExpandedProperty, value);
	}

	public static readonly StyledProperty<object> FooterProperty =
	AvaloniaProperty.Register<ChameleonContentControl, object>(nameof(Footer));
	public object Footer {
		get => GetValue(FooterProperty);
		set => SetValue(FooterProperty, value);
	}

	public static readonly StyledProperty<object> TitleContentProperty =
	AvaloniaProperty.Register<ChameleonContentControl, object>(nameof(TitleContent));
	public object TitleContent {
		get => GetValue(TitleContentProperty);
		set => SetValue(TitleContentProperty, value);
	}
	protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
		base.OnApplyTemplate(e);
		//_optionsHost = e.NameScope.Find<Border>("OptionsRegion");
		//_exampleThemeScopeProvider = e.NameScope.Find<ThemeVariantScope>("ThemeScopeProvider");

		expandOptionsButton = e.NameScope.Find<Button>("ShowHideOptionsButton");
		if (expandOptionsButton is null) return;
		expandOptionsButton.Click += (s, e) => {
			IsOptionsExpanded = !IsOptionsExpanded;
			UpdateIcon();
		};
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
		base.OnPropertyChanged(change);
		if (Options is null) return;
		else if (change.Property == OptionsProperty) PseudoClasses.Set(":options", change.NewValue != null);
		else if (change.Property == BoundsProperty) {
			var wid = change.GetNewValue<Rect>().Width;
			PseudoClasses.Set(":mediumWidth", wid < 690);
			PseudoClasses.Set(":smallWidth", wid < 480);
		}
	}
	private void UpdateIcon() {
		IconShevron = IsOptionsExpanded ? "ChevronUp" : "ChevronDown";
	}
}
