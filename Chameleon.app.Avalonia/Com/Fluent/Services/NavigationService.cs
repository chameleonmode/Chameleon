using Avalonia.Controls;
using Chameleon.lib.Common.Interfaces.Services;

using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Media.Animation;
using FluentAvalonia.UI.Navigation;

namespace Chameleon.Av.Fluent.Common.Services;

public class NavigationService : INavigationService
{
    public void SetFrame(object f)
    {
        _frame = f as Frame;
    }

    public void SetOverlayHost(object p)
    {
        _overlayHost = p as Panel;
    }

    public void Navigate(Type t)
    {
        _frame?.Navigate(t);
    }
    public void NavigateToType(Type t, object? parameter = null)
    {
        NavigateToType(t, parameter, null);
    }
    public void NavigateToType(Type t,object? parameter = null, NavigationTransitionInfo? transitionInfo = null)
    {
        _frame?.NavigateToType(t,  parameter, BuildOptions(transitionInfo));
    }

    public void NavigateFromContext(object dataContext)
    {
        NavigateFromContext(dataContext, null);
    }
    public void NavigateFromContext(object dataContext, NavigationTransitionInfo? transitionInfo = null)
    {
        _frame?.NavigateFromObject(dataContext,BuildOptions(transitionInfo));
    }

    public void ClearOverlay()
    {
        _overlayHost?.Children.Clear();
    }

    FrameNavigationOptions BuildOptions(NavigationTransitionInfo? transitionInfo = null)
    {
        return new FrameNavigationOptions
        {
            IsNavigationStackEnabled = true,
            TransitionInfoOverride = transitionInfo ?? new SuppressNavigationTransitionInfo()
        };
    }

    private Frame? _frame;
    private Panel? _overlayHost;

    public void Pop()
    {
        //TODO: implement other back possibilitys when they come up
        if (_frame?.CanGoBack == true && _frame.Content.GetType().Name == "UserProfileIdentityView")
            _frame?.GoBack();
    }
}