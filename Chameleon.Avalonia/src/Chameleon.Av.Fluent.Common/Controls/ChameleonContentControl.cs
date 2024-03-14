using Avalonia.Controls;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Rendering.Composition.Animations;
using Avalonia.Rendering.Composition;

namespace Chameleon.Av.Fluent.Common.Controls;

public class ChameleonContentControl : HeaderedContentControl
{
    public ChameleonContentControl()
    {
        PseudoClasses.Add(":optionsfull");
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        //_exampleThemeScopeProvider = e.NameScope.Find<ThemeVariantScope>("ThemeScopeProvider");
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        // Do this here rather than OnApplyTemplate, otherwise this will animate
        // on load and that isn't desired
        //AttachOptionsHostAnimation();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        //if (change.Property == BoundsProperty)
        //{
        //    var wid = change.GetNewValue<Rect>().Width;

        //    PseudoClasses.Set(":mediumWidth", wid < 725);
        //    PseudoClasses.Set(":smallWidth", wid < 500);
        //}
    }
}
