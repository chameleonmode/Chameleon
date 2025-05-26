using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;

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
