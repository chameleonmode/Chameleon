using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Rendering.Composition;
using Avalonia.Styling;

using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.lib.Common.Util;
using Chameleon.lib.CommunityToolkit.MvvM;

using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Controls.Experimental;
using FluentAvalonia.UI.Navigation;

namespace Chameleon.Av.Fluent.Common.Pages;

public class ChameleonPageBase : AutoViewModelLocatorControl {
	private bool _isSmallWidth2;
	private bool _hasLoaded;

	private Panel? _detailsPanel;
	private IconSourceElement? _previewImageHost;
	private StackPanel? _detailsHost;
	private ScrollViewer? _scroller;

	public virtual Visual? AnimateVisual { get; set; }

	public ChameleonPageBase()
	{
		SizeChanged += OnSizeChanged;
		AddHandler(Frame.NavigatingFromEvent, OnNavigatingFrom, RoutingStrategies.Direct);
		AddHandler(Frame.NavigatedToEvent, OnNavigatedTo, RoutingStrategies.Direct);
	}

	#region dp
	// 
	public IconSource? PreviewImage {
		get => GetValue(PreviewImageProperty);
		set => SetValue(PreviewImageProperty, value);
	}
	public static readonly StyledProperty<IconSource?> 
		PreviewImageProperty = AvaloniaProperty.Register<ChameleonPageBase, IconSource?>(nameof(PreviewImage));

	// 
	public string ControlName {
		get => GetValue(ControlNameProperty);
		set => SetValue(ControlNameProperty, value);
	}
	public static readonly StyledProperty<string> 
		ControlNameProperty = AvaloniaProperty.Register<ChameleonPageBase, string>(nameof(ControlName));

	//
	public string Description {
		get => GetValue(DescriptionProperty);
		set => SetValue(DescriptionProperty, value);
	}
	public static readonly StyledProperty<string>
		DescriptionProperty = AvaloniaProperty.Register<ChameleonPageBase, string>(nameof(Description));
	#endregion

	#region overrides
	protected override Type StyleKeyOverride => typeof(ChameleonPageBase);

  protected override void OnLoaded(RoutedEventArgs e)
  {
    _hasLoaded = true;
    if (_detailsPanel == null)
      return;

    var ec = ElementComposition.GetElementVisual(_detailsPanel);
    if (ec?.Compositor == null)
      return;

    var offsetAnimation = ec.Compositor.CreateVector3KeyFrameAnimation();
    if (offsetAnimation == null)
      return;

    offsetAnimation.Target = "Offset";
    offsetAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
    offsetAnimation.Duration = TimeSpan.FromMilliseconds(250);

    var ani = ec.Compositor.CreateImplicitAnimationCollection();
    ani["Offset"] = offsetAnimation;

    ec.ImplicitAnimations = ani;
    base.OnLoaded(e);
  }

	protected override void OnUnloaded(RoutedEventArgs e)
	{
		base.OnUnloaded(e);
		_hasLoaded = false;
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		_previewImageHost = e.NameScope.Find<IconSourceElement>("PreviewImageElement");
		_detailsHost = e.NameScope.Find<StackPanel>("DetailsTextHost");
		_detailsPanel = e.NameScope.Find<Panel>("PageDetails");
		_scroller = e.NameScope.Find<ScrollViewer>("PageScroller");
	}
	#endregion

	private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
	{
		var sz = e.NewSize.Width;

		var isSmallWidth2 = sz < 580;

		PseudoClasses.Set(":smallWidth", sz < 710);
		PseudoClasses.Set(":smallWidth2", isSmallWidth2);

		async Task AnimateOptions(bool toSmall)
		{
			if (!await TaskUtil.AwaitFor(() => _hasLoaded))
				return;

			double x = toSmall ? 70 : -70;
			double y = toSmall ? -30 : 30;
			_ = new Animation {
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
		}

		if (isSmallWidth2 && !_isSmallWidth2) {
			_ = AnimateOptions(true);
			_isSmallWidth2 = true;
		} else if (!isSmallWidth2 && _isSmallWidth2) {
			_ = AnimateOptions(false);
			_isSmallWidth2 = false;
		}
	}
	
	private async void OnNavigatingFrom(object? sender, NavigatingCancelEventArgs e)
	{
		if (_previewImageHost == null)
			return;

		// Only setup the ConnectedAnimation if it makes sense
		_ = await TaskUtil.TryAwaitFor(async () => {
			var svc = ConnectedAnimationService.GetForView(TopLevel.GetTopLevel(this));
			_ = svc.PrepareToAnimate("BackAnimation", await TaskUtil.AwaitFor(() => AnimateVisual != null, 1) ? AnimateVisual : _previewImageHost.Parent as Control);
		}, 2);
	}
	
	private async void OnNavigatedTo(object? sender, NavigationEventArgs e)
	{
		if (DataContext is ViewModelObjectBase pageViewModel) {
			await pageViewModel.OnNavigatedToAsync(e.Parameter);
			ControlName ??= pageViewModel.Title ?? "xxx";
		}

		var svc = await TaskUtil.TryAwaitFor(() => ConnectedAnimationService.GetForView(TopLevel.GetTopLevel(this)), 2);   //TODO: might crash if wrong page
		if (svc is null)
			return;

		var animation = svc.GetAnimation("ForwardAnimation");

		if (animation != null) {
			// PreviewImageHost is inside a Viewbox which can really mess with the Composition 
			// animation - use the viewbox directly for the animation to ensure it works correctly
			if (await TaskUtil.AwaitFor(() => AnimateVisual != null, 1)) {
				if (_detailsPanel != null)
					_detailsPanel.IsVisible = false;

				_ = animation.TryStart(AnimateVisual, [_scroller]);
			} else {
				_ = animation.TryStart(_previewImageHost?.Parent as Control, [_detailsHost, _scroller]);
			}
		}
	}
}
