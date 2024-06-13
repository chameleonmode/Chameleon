using Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia;
using Avalonia.Media.Imaging;
using FluentAvalonia.UI.Controls;

namespace Chameleon.Av.Fluent.Common.Controls;

public enum PageHeaderTextType
{
    Dashboard,
    Profiles,
    Automation,
    Main,
    CoreControls,
    FAControls,
    Settings
}

public class PageHeaderControl : TemplatedControl
{
    public PageHeaderControl()
    {
        SizeChanged += OnSizeChanged;
        ActualThemeVariantChanged += OnActualThemeVariantChanged;
    }

    public static readonly DirectProperty<PageHeaderControl, PageHeaderTextType> TextTypeProperty =
        AvaloniaProperty.RegisterDirect<PageHeaderControl, PageHeaderTextType>(nameof(TextType),
            x => x.TextType, (x, v) => x.TextType = v);

    public PageHeaderTextType TextType
    {
        get => _textType;
        set => SetAndRaise(TextTypeProperty, ref _textType, value);
    }

    public static readonly DirectProperty<PageHeaderControl, Uri> TitleTextImageProperty =
        AvaloniaProperty.RegisterDirect<PageHeaderControl, Uri>(nameof(TitleTextImage),
            x => x.TitleTextImage, (x, v) => x.TitleTextImage = v);

    public Uri TitleTextImage
    {
        get => _titleTextImage;
        set => SetAndRaise(TitleTextImageProperty, ref _titleTextImage, value);
    }

    public static readonly DirectProperty<PageHeaderControl, object> ContentObjectProperty =
        AvaloniaProperty.RegisterDirect<PageHeaderControl, object>(nameof(ContentObject),
        x => x.ContentObject, (x, v) => x.ContentObject = v);
    public object ContentObject
    {
        get => _contentObject;
        set => SetAndRaise(ContentObjectProperty, ref _contentObject, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _text1 = e.NameScope.Get<Image>("TitleTextImageHost");
        _fontIcon = e.NameScope.Get<FontIcon>("TitleTextFontHost");
        UpdateTitleText();
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        var wid = e.NewSize.Width;
        //if (wid < 630)
        //{
        //    var delta = 630 - wid;

        //    _text1.Width = 400 - delta;
        //}
        //else
        //{
        //    _text1.Width = double.NaN;
        //}
        //PseudoClasses.Set(":small", wid < 450);
        //

        //_text1.Width = 180;
    }

    private void UpdateTitleText()
    {
        if (_text1 == null)
            return;

        var theme = ActualThemeVariant;

        //const string asset = "avares://Chameleon.Avalonia.Common/Assets/Images/";

        _fontIcon.Glyph = TextType switch
        {
            PageHeaderTextType.Dashboard => "Dashboard",
            PageHeaderTextType.Automation => "Automation",
            PageHeaderTextType.Profiles => "Profiles & Folders",   //removeing  & Folders might feel better should test out in future
            PageHeaderTextType.Settings => "Settings",
            _ => "Chameleon"
        };

        //if (theme == ThemeVariant.Light)
        //{
        //    _fontIcon.Fo
        //}

        //header += ".png";

        //using var s = AssetLoader.Open(new Uri($"{asset}{header}"));
        using var s = AssetLoader.Open(new Uri($"avares://Chameleon.Avalonia.Common/Assets/Images/logo-merge.png"));
        _text1.Source = new Bitmap(s);
    }

    private void OnActualThemeVariantChanged(object? sender, EventArgs e)
    {
        UpdateTitleText();
    }

    private object _contentObject;
    private Uri _titleTextImage;
    private Image _text1;
    private FontIcon _fontIcon;
    private PageHeaderTextType _textType;
}
