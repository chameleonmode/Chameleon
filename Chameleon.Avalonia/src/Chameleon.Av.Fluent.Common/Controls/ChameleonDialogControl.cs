using Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia;
using FluentAvalonia.UI.Controls;

namespace Chameleon.Av.Fluent.Common.Controls;

public class ChameleonDialogControl : HeaderedContentControl
{

    ///// <summary>
    ///// Defines the <see cref="PrimaryButtonText"/> property
    ///// </summary>
    //public static readonly StyledProperty<string> PrimaryButtonTextProperty =
    //    AvaloniaProperty.Register<ChameleonDialogControl, string>(nameof(PrimaryButtonText));
    ///// <summary>
    ///// Gets or sets the text to display on the primary button.
    ///// </summary>
    //public string PrimaryButtonText
    //{
    //    get => GetValue(PrimaryButtonTextProperty);
    //    set => SetValue(PrimaryButtonTextProperty, value);
    //}


    ///// <summary>
    ///// Defines the <see cref="SecondaryButtonText"/> property
    ///// </summary>
    //public static readonly StyledProperty<string> SecondaryButtonTextProperty =
    //    AvaloniaProperty.Register<ContentDialog, string>(nameof(SecondaryButtonText));
    ///// <summary>
    ///// Gets or sets the text to be displayed on the secondary button.
    ///// </summary>
    //public string SecondaryButtonText
    //{
    //    get => GetValue(SecondaryButtonTextProperty);
    //    set => SetValue(SecondaryButtonTextProperty, value);
    //}


    ///// <summary>
    ///// Defines the <see cref="CloseButtonText"/> property
    ///// </summary>
    //public static readonly StyledProperty<string> CloseButtonTextProperty =
    //    AvaloniaProperty.Register<ChameleonDialogControl, string>(nameof(CloseButtonText));
    ///// <summary>
    ///// Gets or sets the text to display on the close button.
    ///// </summary>
    //public string CloseButtonText
    //{
    //    get => GetValue(CloseButtonTextProperty);
    //    set => SetValue(CloseButtonTextProperty, value);
    //}

    public ChameleonDialogControl()
    {
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
    }
}
