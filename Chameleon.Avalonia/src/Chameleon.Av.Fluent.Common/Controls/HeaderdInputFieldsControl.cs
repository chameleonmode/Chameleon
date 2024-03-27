using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using FluentAvalonia.UI.Controls;

namespace Chameleon.Av.Fluent.Common.Controls;

[TemplatePart("TextBoxesHost", typeof(ItemsPresenter))]
public class HeaderdInputFieldsControl : HeaderedContentControl
{
    public HeaderdInputFieldsControl()
    {
        _textboxes = new List<TextBox>();
    }
    public static readonly StyledProperty<string?> TitleProperty =
    AvaloniaProperty.Register<HeaderdInputFieldsControl, string?>(nameof(Title));

    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<HeaderdInputFieldsControl, string?>(nameof(Text),null,false,BindingMode.TwoWay);
    //AvaloniaProperty.RegisterDirect<HeaderdInputFieldsControl, string?>(
    //        nameof(Text),
    //    x => x.Text, 
    //    (x, v) => x.Text = v,
    //     null,
    //      BindingMode.TwoWay);
    //AvaloniaProperty.Register<HeaderdInputFieldsControl, string?>(nameof(Text));

    public static readonly StyledProperty<string?> TitleDescriptionProperty =
    AvaloniaProperty.Register<HeaderdInputFieldsControl, string?>(nameof(TitleDescription));

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }


    //public string? Text
    //{
    //    get => _text;
    //    set => SetAndRaise(TextProperty, ref _text, value);
    //}
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

    public static readonly DirectProperty<HeaderdInputFieldsControl, IList<TextBox>> TextBoxesProperty =
    AvaloniaProperty.RegisterDirect<HeaderdInputFieldsControl, IList<TextBox>>(nameof(TextBoxes),
        x => x.TextBoxes, (x, v) => x.TextBoxes = v);

    /// <summary>
    /// Gets the list of TextBox that display at the bottom of the TaskDialog
    /// </summary>
    public IList<TextBox> TextBoxes
    {
        get => _textboxes;
        set => SetAndRaise(TextBoxesProperty, ref _textboxes, value);
    }

    private string? _text;
    private IList<TextBox> _textboxes;
}
