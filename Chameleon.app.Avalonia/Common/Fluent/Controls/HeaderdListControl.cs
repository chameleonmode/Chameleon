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

public class HeaderdListControl : HeaderedContentControl
{
    public HeaderdListControl()
    {
        _faComboBoxes = new List<HeaderedContentControl>();
    }
    //public static readonly StyledProperty<string?> TitleProperty =
    //    AvaloniaProperty.Register<HeaderdInputFieldsControl, string?>(nameof(Title));

    //public static readonly StyledProperty<string?> TextProperty =
    //    AvaloniaProperty.Register<HeaderdInputFieldsControl, string?>(nameof(Text),null,false,BindingMode.TwoWay);

    //public static readonly StyledProperty<string?> TitleDescriptionProperty =
    //    AvaloniaProperty.Register<HeaderdInputFieldsControl, string?>(nameof(TitleDescription));

    public static readonly DirectProperty<HeaderdListControl, IList<HeaderedContentControl>> HeaderdListProperty =
        AvaloniaProperty.RegisterDirect<HeaderdListControl, IList<HeaderedContentControl>>(nameof(HeaderdList),
        x => x.HeaderdList, (x, v) => x.HeaderdList = v);
    //public string? Title
    //{
    //    get => GetValue(TitleProperty);
    //    set => SetValue(TitleProperty, value);
    //}

    //public string? Text
    //{
    //    get => GetValue(TextProperty);
    //    set => SetValue(TextProperty, value);
    //}

    //public string? TitleDescription
    //{
    //    get => GetValue(TitleDescriptionProperty);
    //    set => SetValue(TitleDescriptionProperty, value);
    //}

    /// <summary>
    /// Gets the list of TextBox that display at the bottom of the TaskDialog
    /// </summary>
    public IList<HeaderedContentControl> HeaderdList
    {
        get => _faComboBoxes;
        set => SetAndRaise(HeaderdListProperty, ref _faComboBoxes, value);
    }

    private IList<HeaderedContentControl> _faComboBoxes;
}
