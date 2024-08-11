using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Av.Fluent.Common.Pages;
using Chameleon.Avalonia.Controls.Settings.ViewModels;
using Chameleon.Common.Helpers;
using Chameleon.Core.Attributes;
using Chameleon.Core.Util;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.Dashboard;
using Chameleon.Interfaces.Settings;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Controls.Experimental;
using FluentAvalonia.UI.Navigation;
using System.Diagnostics;

namespace Chameleon.Avalonia.Controls.Settings;

public partial class SettingsView : ChameleonNavigationPage
        , ISettingsView
{
    public SettingsView()
    {
        InitializeComponent();
        //DataContext = ContainerServiceHelper.Resolve<ISettingsViewModel>();

        LaunchSupportLinkItem.Click += LaunchSupportLinkItemClick;
    }

    private void LaunchSupportLinkItemClick(object sender, RoutedEventArgs e)
    {
        var uri = new Uri("https://help.chameleonmode.com");
        try
        {
            ProUtil.GoToUrlDefault(uri.ToString());
        }
        catch
        {
             ToasterHelper.ShowErr($"Error navigationg to {uri}");
        }
    }

    //private void Default_Tapped(object? sender, TappedEventArgs e)
    //{
    //    if (e.Source is Visual v)
    //    {
    //        var lbi = v.FindAncestorOfType<SettingsExpander>(true);
    //        if (lbi != null && lbi.DataContext is SubPageViewControl fci)
    //        {
    //            var item = lbi.GetVisualDescendants()
    //                .Where(x => x is Viewbox && x.Name == "IconHost")
    //                .FirstOrDefault();
    //            _animationPage = fci;


    //        }
    //    }
    //}

    //private void OnNavigatedTo(object sender, NavigationEventArgs e)
    //{
    //    if (_animationPage == null)
    //        return;

    //    var svc = ConnectedAnimationService.GetForView(TopLevel.GetTopLevel(this));
    //    var anim = svc.GetAnimation("BackAnimation");

    //    if (anim == null)
    //        return;

    //    var item = this.GetVisualDescendants()
    //                .Where(x => x is SettingsExpander && (x as ICommandSource).CommandParameter == _animationPage)
    //                .FirstOrDefault()
    //                .GetVisualDescendants()
    //                .Where(x => x is Viewbox && x.Name == "IconHost")
    //                .FirstOrDefault();
    //    var presenter = item;// GetAnimationSource();

    //    // In WinUI, ConnectedAnimation is somehow exempt from all clipping behaviors
    //    // Here, we are not, so disable ClipToBounds on all elements in the SettingsExpander
    //    // The rest are taken care of in the xaml.
    //    // NOTE: The ScrollViewer is not changed here as that's important for scrolling - thus
    //    // the animation will be cut off, but the back animation is pretty fast and mostly is
    //    // only visible closer to the element so we're ok, I think
    //    var x = presenter.GetVisualParent();
    //    while (!(x is ScrollContentPresenter) && x != null)
    //    {
    //        x.ClipToBounds = false;
    //        x = x.GetVisualParent();
    //    }

    //    anim.Configuration = new DirectConnectedAnimationConfiguration();
    //    anim.TryStart(presenter);
    //}

    //private void OnNavigatingFrom(object sender, NavigatingCancelEventArgs e)
    //{
    //    _animationPage = e.Parameter;
    //    if (_animationPage == null)
    //        return;

    //    //// We're not navigating to a control page, don't set up the animation & clear
    //    //// the previous animation source
    //    //if (!e.SourcePageType.Name.Equals(nameof(SubPageViewControl)))
    //    //{
    //    //    _animationPage = null;
    //    //    _animationPage = null;
    //    //    return;
    //    //}

    //    var item = this.GetVisualDescendants()
    //                .Where(x => x is SettingsExpander && (x as ICommandSource).CommandParameter == _animationPage)
    //                .FirstOrDefault()
    //                .GetVisualDescendants()
    //                .Where(x => x is Viewbox && x.Name == "IconHost")
    //                .FirstOrDefault(); 
    //    var svc = ConnectedAnimationService.GetForView(TopLevel.GetTopLevel(this));
    //    svc.PrepareToAnimate("ForwardAnimation", item);
    //}

    public void SetTabContent(SettingTabs tab)
    {
        throw new NotImplementedException();
    }

    //private object? _animationPage;

    //private void SettingsExpander_Tapped(object? sender, TappedEventArgs e)
    //{
    //}
}