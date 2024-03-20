using Avalonia;
using Avalonia.Controls.Primitives;

namespace Chameleon.Av.Fluent.Common.Controls;

public class SvgIconControl  : TemplatedControl
{
    public static readonly StyledProperty<string?> IconNameProperty =
    AvaloniaProperty.Register<SvgIconControl, string?>(nameof(IconName));

    public string? IconName
    {
        get => GetValue(IconNameProperty);
        set => SetValue(IconNameProperty, value);
    }
}
