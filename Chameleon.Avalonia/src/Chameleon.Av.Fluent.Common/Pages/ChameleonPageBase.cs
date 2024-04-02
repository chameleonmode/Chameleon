using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Rendering.Composition;
using Avalonia.Styling;
using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Av.Fluent.Common.Services;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.Services;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Controls.Experimental;
using FluentAvalonia.UI.Navigation;
using System;
using System.Runtime.Intrinsics.X86;

namespace Chameleon.Av.Fluent.Common.Pages;

public class ChameleonPageBase : AutoViewModelLocatorControl
{
    private CancellationTokenSource? _cts;
    private bool _isSmallWidth2;
    private bool _hasLoaded;

    private Button? _toggleThemeButton;
    private Panel? _detailsPanel;
    //private StackPanel? _optionsHost;
    private IconSourceElement? _previewImageHost;
    private StackPanel? _detailsHost;
    private ScrollViewer? _scroller;


    public ChameleonPageBase()
    {
        SizeChanged += ControlsPageBaseSizeChanged;
        AddHandler(Frame.NavigatingFromEvent, FrameNavigatingFrom, RoutingStrategies.Direct);
        AddHandler(Frame.NavigatedToEvent, FrameNavigatedTo, RoutingStrategies.Direct);
    }

    #region dp
    public static readonly StyledProperty<IconSource> PreviewImageProperty = 
        AvaloniaProperty.Register<ChameleonPageBase, IconSource>(nameof(PreviewImage));
    public IconSource PreviewImage
    {
        get => GetValue(PreviewImageProperty);
        set => SetValue(PreviewImageProperty, value);
    }

    public static readonly StyledProperty<string> ControlNameProperty =
    AvaloniaProperty.Register<ChameleonPageBase, string>(nameof(ControlName));
    public string ControlName
    {
        get => GetValue(ControlNameProperty);
        set => SetValue(ControlNameProperty, value);
    }

    public static readonly StyledProperty<string> DescriptionProperty =
    AvaloniaProperty.Register<ChameleonPageBase, string>(nameof(Description));
    public string Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
    #endregion

    //protected ThemeVariantScope? ThemeScopeProvider { get; private set; }

    #region overrides
    protected override Type StyleKeyOverride => typeof(ChameleonPageBase);

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        _hasLoaded = true;
        SetDetailsAnimation();
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        _hasLoaded = false;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        //ThemeScopeProvider = e.NameScope.Find<ThemeVariantScope>("ThemeScopeProvider");

        _previewImageHost = e.NameScope.Find<IconSourceElement>("PreviewImageElement");
        _detailsHost = e.NameScope.Find<StackPanel>("DetailsTextHost");
        //_optionsHost = e.NameScope.Find<StackPanel>("OptionsRegion");
        _detailsPanel = e.NameScope.Find<Panel>("PageDetails");
        _scroller = e.NameScope.Find<ScrollViewer>("PageScroller");
    }                 
    private void SetDetailsAnimation()
    {
        var ec = ElementComposition.GetElementVisual(_detailsPanel);
        var compositor = ec.Compositor;

        var offsetAnimation = compositor.CreateVector3KeyFrameAnimation();
        offsetAnimation.Target = "Offset";
        offsetAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
        offsetAnimation.Duration = TimeSpan.FromMilliseconds(250);

        var ani = compositor.CreateImplicitAnimationCollection();
        ani["Offset"] = offsetAnimation;

        ec.ImplicitAnimations = ani;
    }
    #endregion


    private void ControlsPageBaseSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var sz = e.NewSize.Width;

        bool isSmallWidth2 = sz < 580;

        PseudoClasses.Set(":smallWidth", sz < 710);
        PseudoClasses.Set(":smallWidth2", isSmallWidth2);

        if (isSmallWidth2 && !_isSmallWidth2)
        {
            AnimateOptions(true);
            _isSmallWidth2 = true;
        }
        else if (!isSmallWidth2 && _isSmallWidth2)
        {
            AnimateOptions(false);
            _isSmallWidth2 = false;
        }
    }
    private async void AnimateOptions(bool toSmall)
    {
        if (!_hasLoaded)
            await Task.Delay(1000);
        if (!_hasLoaded)
            return;

            _cts?.Cancel();

        _cts = new CancellationTokenSource();
        double x = toSmall ? 70 : -70;
        double y = toSmall ? -30 : 30;
        var ani = new Animation
        {
            Duration = TimeSpan.FromSeconds(0.25),
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0d),
                    Setters =
                    {
                        new Setter(TranslateTransform.XProperty, x),
                        new Setter(TranslateTransform.YProperty, y),
                        new Setter(OpacityProperty, 0d)
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1d),
                    Setters =
                    {
                        new Setter(TranslateTransform.XProperty, 0d),
                        new Setter(TranslateTransform.YProperty, 0d),
                        new Setter(OpacityProperty, 1d)
                    },
                    KeySpline = new KeySpline(0, 0, 0, 1)
                }
            }
        };

        //if(_optionsHost != null)
        //    await ani.RunAsync(_optionsHost, _cts.Token);

        _cts = null;
    }

    private void FrameNavigatingFrom(object sender, NavigatingCancelEventArgs e)
    {
        //If TargetType is not set, we know we're currently on a CoreControls page since those
        // are grouped pages - whereas, FA controls only display one control per page and
        // set all the extra properties
       //bool isFAControlPage = TargetType != null;

        // Only setup the ConnectedAnimation if it makes sense
        //if ((!isFAControlPage && e.SourcePageType == typeof(CoreControlsPageViewModel)) ||
        //    (isFAControlPage && e.SourcePageType == typeof(FAControlsOverviewPageViewModel)))
        //{
        //    // Only setup the Back connected animation if we're going back to the
        //    // controls list pages
            var svc = ConnectedAnimationService.GetForView(TopLevel.GetTopLevel(this));
            svc.PrepareToAnimate("BackAnimation", (Control)_previewImageHost.Parent);
        ContainerServiceHelper.Resolve<INavigationService>().PreviousPage = this;
        //}
    }

    private async void FrameNavigatedTo(object sender, NavigationEventArgs e)
    {
        if (DataContext is ISubPageViewModel pageViewModel)
        {
            await pageViewModel.OnNavigatedToAsync(e.Parameter);
            ControlName = pageViewModel.Title;
        }

        var svc = ConnectedAnimationService.GetForView(TopLevel.GetTopLevel(this));
        var animation = svc.GetAnimation("ForwardAnimation");

        if (animation != null)
        {
            var coordinated = new List<Visual>
            {
                //_optionsHost,
                _detailsHost,
                _scroller
            };

            // PreviewImageHost is inside a Viewbox which can really mess with the Composition 
            // animation - use the viewbox directly for the animation to ensure it works correctly
            animation.TryStart((Control)_previewImageHost.Parent, coordinated);

        }
    }
}
