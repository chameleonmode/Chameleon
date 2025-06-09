using Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia;
using Avalonia.Media.Imaging;
using FluentAvalonia.UI.Controls;

namespace Chameleon.client.UI.Controls;

public class PageHeader : TemplatedControl {
	public enum HeaderType {
		Dashboard,
		Profiles,
		Automation,
		Main,
		Settings,
		FunctionalSettings
	}
	private object? _contentObject;
	private Uri? _titleTextImage;
	private Image? _text1;
	private FontIcon? _fontIcon;
	private HeaderType _textType = HeaderType.Dashboard;

	public PageHeader() {
		ActualThemeVariantChanged += OnActualThemeVariantChanged;
	}

	public static readonly DirectProperty<PageHeader, HeaderType> TextTypeProperty = AvaloniaProperty.RegisterDirect<PageHeader, HeaderType>(nameof(TextType), x => x.TextType, (x, v) => x.TextType = v);
	public HeaderType TextType {
		get => _textType;
		set => SetAndRaise(TextTypeProperty, ref _textType, value);
	}

	public static readonly DirectProperty<PageHeader, Uri?> TitleTextImageProperty = AvaloniaProperty.RegisterDirect<PageHeader, Uri?>(nameof(TitleTextImage), x => x.TitleTextImage, (x, v) => x.TitleTextImage = v);
	public Uri? TitleTextImage {
		get => _titleTextImage;
		set => SetAndRaise(TitleTextImageProperty, ref _titleTextImage, value);
	}

	public static readonly DirectProperty<PageHeader, object?> ContentObjectProperty = AvaloniaProperty.RegisterDirect<PageHeader, object?>(nameof(ContentObject), x => x.ContentObject, (x, v) => x.ContentObject = v);
	public object? ContentObject {
		get => _contentObject;
		set => SetAndRaise(ContentObjectProperty, ref _contentObject, value);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
		base.OnApplyTemplate(e);

		_text1 = e.NameScope.Get<Image>("TitleTextImageHost");
		_fontIcon = e.NameScope.Get<FontIcon>("TitleTextFontHost");
		UpdateTitleText();
	}

	private void UpdateTitleText() {
		if (_text1 == null || _fontIcon == null)
			return;

		_fontIcon.Glyph = TextType switch {
			HeaderType.Dashboard => "Dashboard",
			HeaderType.Automation => "Automation",
			HeaderType.Profiles => "Profiles",
			HeaderType.Settings => "Settings",
			HeaderType.FunctionalSettings => "General",
			_ => "Chameleon"
		};

		using var s = AssetLoader.Open(new Uri($"avares://Chameleon.app.Avalonia/Assets/Images/logo-merge.png"));
		_text1.Source = new Bitmap(s);
	}

	private void OnActualThemeVariantChanged(object? sender, EventArgs e) {
		UpdateTitleText();
	}
}
