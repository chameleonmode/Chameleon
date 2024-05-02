using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Av.Fluent.Common.Services;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.Dashboard;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Controls.Experimental;
using FluentAvalonia.UI.Navigation;

namespace Chameleon.Av.Fluent.Common.Pages;

public class ChameleonNavigationPage : AutoViewModelLocatorControl
{
    public ChameleonNavigationPage()
    {
        // Use the frame events here to ensure ConnectedAnimations still work with
        // Back/Forward navigation and not just explicit page invokes
        AddHandler(Frame.NavigatingFromEvent, OnNavigatingFrom, RoutingStrategies.Direct);
        AddHandler(Frame.NavigatedToEvent, OnNavigatedTo, RoutingStrategies.Direct);

        //Tapped += OnPageTapped;
        DoubleTapped += OnPageTapped;
    }

    private void OnPageTapped(object sender, TappedEventArgs e)
    {
        if (e.Source is Visual v)
        {
            if (v.FindAncestorOfType<Button>(true) is null &&
                v.FindAncestorOfType<ListBoxItem>(true) is ListBoxItem lbi && 
                lbi.DataContext is IUserProfileViewModelBase up)
            {
                //var item = lbi.GetVisualDescendants()
                //    .Where(x => x is Viewbox && x.Name == "IconHost")
                //    .FirstOrDefault();
                //_animationPage = fci;

                //NavigationService.Instance.NavigateFromContext(fci);
                up.Open();
            }
        }
    }

    private async void OnNavigatedTo(object sender, NavigationEventArgs e)
    {
        if (DataContext is IPageViewModel pageViewModel)
        {
            await pageViewModel.OnNavigatedToAsync(e.Parameter);
        }
        if (_animationPage == null || _animationPageParent == null)
            return;

        var svc = ConnectedAnimationService.GetForView(TopLevel.GetTopLevel(this));
        var anim = svc.GetAnimation("BackAnimation");

        if (anim == null)
            return;

        GetNavAnimationVisuals(_navParam);

        if (_animationPage == null) return;

        // In WinUI, ConnectedAnimation is somehow exempt from all clipping behaviors
        // Here, we are not, so disable ClipToBounds on all elements in the SettingsExpander
        // The rest are taken care of in the xaml.
        // NOTE: The ScrollViewer is not changed here as that's important for scrolling - thus
        // the animation will be cut off, but the back animation is pretty fast and mostly is
        // only visible closer to the element so we're ok, I think
        var x = _animationPage.GetVisualParent();
        while (x is not ScrollContentPresenter && x != null)
        {
            x.ClipToBounds = false;
            x = x.GetVisualParent();
        }

        anim.Configuration = new DirectConnectedAnimationConfiguration();
        anim.TryStart(_animationPage);
    }

    private void OnNavigatingFrom(object sender, NavigatingCancelEventArgs e)
    {
        _navParam = e.Parameter;

        GetNavAnimationVisuals(_navParam);

        if (_animationPage is not null)
        {
            var svc = ConnectedAnimationService.GetForView(TopLevel.GetTopLevel(this));
            try
            {
                svc.PrepareToAnimate("ForwardAnimation", _animationPage);
            }
            catch (Exception ex)
            {
                svc.GetAnimation("ForwardAnimation");
                _animationPage = _animationPageParent = null;
            }
        }
    }

    private void GetNavAnimationVisuals(object navParam)
    {
        _animationPage = _animationPageParent = null;

        if (navParam is null) return;
        else if (navParam is not null and string command)
        {
            _animationPageParent = this.GetVisualDescendants()?.Where(x => (x as ICommandSource)?.CommandParameter is string cmd && cmd == command)?.FirstOrDefault();
            _animationPage = _animationPageParent?.GetVisualDescendants()?.Where(x => x is Viewbox && x.Name == "IconHost")?.FirstOrDefault();
        }
        else
        {
            if (navParam is IUserProfile iprofile)
            {
                _animationPageParent = this.GetVisualDescendants()?
                    .Where(x => x is ListBox && x.Name == "lbProfiles")?
                    .FirstOrDefault();
                //if (this is IDashboardView)
                //    _animationPage = _animationPageParent?.GetVisualDescendants()?
                //        .Where(x => x is Viewbox b && b.Tag == iprofile)?
                //        .FirstOrDefault();
                //else
                    _animationPage = _animationPageParent?.GetVisualDescendants()?
                        .Where(x => x is ListBoxItem b && b.DataContext is IUserProfileViewModelBase dc && dc.UserProfile == iprofile)?
                        .FirstOrDefault();
                if (_animationPage == null && _animationPageParent is ListBox l) // && l.Items.Count == 1
                    if(l.Items.Count >= 10)
                    _animationPage = _animationPageParent?.GetVisualDescendants()?.Where(x => x is ListBoxItem b && b.DataContext is IUserProfileViewModelBase)?
                        .FirstOrDefault();
                if (_animationPage == null)
                    _animationPage = _animationPageParent;


            }
            else if (navParam is IUserProfileFolder)
            {
                _animationPageParent = this.GetVisualDescendants()
                    .Where(x => x is IUserProfileFoldersView)?
                    .FirstOrDefault();
                _animationPage = _animationPageParent?.GetVisualDescendants()?
                     .Where(x => x is Viewbox && x.Name == "IconHost" && (x as Control).Tag == navParam)?
                     .FirstOrDefault();
            }
        }
    }

    private Visual? _animationPageParent;
    private Visual? _animationPage;
    private object? _navParam;

}
