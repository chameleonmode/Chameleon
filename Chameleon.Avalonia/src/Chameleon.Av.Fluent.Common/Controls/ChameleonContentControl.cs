using Avalonia.Controls;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Rendering.Composition.Animations;
using Avalonia.Rendering.Composition;
using Chameleon.Avalonia.Fluent.Common.Base;

namespace Chameleon.Av.Fluent.Common.Controls;

public class ChameleonContentControl : HeaderedContentControl
{
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<ChameleonContentControl, string?>(nameof(Title));

    public static readonly StyledProperty<string?> TitleDescriptionProperty =
    AvaloniaProperty.Register<ChameleonContentControl, string?>(nameof(TitleDescription));

    public static readonly StyledProperty<Control> OptionsProperty =
        AvaloniaProperty.Register<ChameleonContentControl, Control>(nameof(Options));

    public static readonly StyledProperty<bool> IsOptionsExpandedProperty =
    AvaloniaProperty.Register<ChameleonContentControl, bool>(nameof(IsOptionsExpanded), true);

    public static readonly StyledProperty<object> FooterProperty =
    AvaloniaProperty.Register<ChameleonContentControl, object>(nameof(Footer));

    public static readonly StyledProperty<object> TitleContentProperty =
        AvaloniaProperty.Register<ChameleonContentControl, object>(nameof(TitleContent));

    public static readonly StyledProperty<string> IconShevronProperty =
       AvaloniaProperty.Register<ChameleonContentControl, string>(nameof(IconShevron));

    public string IconShevron
    {
        get => GetValue(IconShevronProperty);
        set => SetValue(IconShevronProperty, value);
    }

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string? TitleDescription
    {
        get => GetValue(TitleDescriptionProperty);
        set => SetValue(TitleDescriptionProperty, value);
    }

    public Control Options
    {
        get => GetValue(OptionsProperty);
        set => SetValue(OptionsProperty, value);
    }

    public bool IsOptionsExpanded
    {
        get => GetValue(IsOptionsExpandedProperty);
        set => SetValue(IsOptionsExpandedProperty, value);
    }

    public object Footer
    {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    public object TitleContent
    {
        get => GetValue(TitleContentProperty);
        set => SetValue(TitleContentProperty, value);
    }
    public ChameleonContentControl()
    {
        PseudoClasses.Add(":optionsfull");

        AttachedToVisualTree += OnAttachedToVisualTree;
    }

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (VisualRoot is not Window window)
        {
            return;
        }
        window
            .GetObservable(Window.ClientSizeProperty)
            .Subscribe(OnWindowSizeChanged);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        //_optionsHost = e.NameScope.Find<Border>("OptionsRegion");
        //_exampleThemeScopeProvider = e.NameScope.Find<ThemeVariantScope>("ThemeScopeProvider");

        _expandOptionsButton = e.NameScope.Find<Button>("ShowHideOptionsButton");
        _expandOptionsButton.Click += OnExpandOptionsClick;
    }

    private void OnWindowSizeChanged(Size newSize)
    {
        bool isWindowChange = newSize.Width < ResponsiveConstants.MaxWindowWidth1060;

        _expandOptionsButton?.SetValue(IsVisibleProperty, isWindowChange);

        IsOptionsExpanded = !isWindowChange;

        UpdateIcon();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        // Do this here rather than OnApplyTemplate, otherwise this will animate
        // on load and that isn't desired
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == OptionsProperty)
        {
            PseudoClasses.Set(":options", change.NewValue != null);
        }
        else if (change.Property == BoundsProperty)
        {
            var wid = change.GetNewValue<Rect>().Width;

            PseudoClasses.Set(":mediumWidth", wid < 725);
            PseudoClasses.Set(":smallWidth", wid < 500);
        }
        //else if (change.Property == IsOptionsExpandedProperty)
        //{
        //    PseudoClasses.Set(":optionsfull", change.GetNewValue<bool>());
        //}
    }


    private void OnExpandOptionsClick(object sender, RoutedEventArgs e)
    {
        IsOptionsExpanded = !IsOptionsExpanded;

        UpdateIcon();
    }

    private void UpdateIcon()
    {
        IconShevron = IsOptionsExpanded ? "ChevronUp" : "ChevronDown";
    }

    private Button _expandOptionsButton;
}
