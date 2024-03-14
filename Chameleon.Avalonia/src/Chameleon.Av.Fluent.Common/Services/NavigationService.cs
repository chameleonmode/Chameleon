using Avalonia.Controls;
using Chameleon.Av.Fluent.Common.Models;
using Chameleon.Av.Fluent.Common.Pages;
using Chameleon.Avalonia.Common.Helpers;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.Dashboard;
using Chameleon.Interfaces.Ioc;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Controls.Primitives;
using FluentAvalonia.UI.Media.Animation;
using System.Configuration;

namespace Chameleon.Av.Fluent.Common.Services;

public class NavigationService
{
    private NavigationService()
    {
            
    }
    public static NavigationService Instance { get; } = new NavigationService();

    public Control? PreviousPage { get; set; }
    public NavigationFactory? NavFactory { get; } = new NavigationFactory();

    public void SetFrame(Frame f)
    {
        _frame = f;
    }

    public void SetOverlayHost(Panel p)
    {
        _overlayHost = p;
    }

    public void Navigate(Type t)
    {
        _frame?.Navigate(t);
    }

    public void NavigateFromContext(object dataContext, NavigationTransitionInfo transitionInfo = null)
    {
        _frame?.NavigateFromObject(dataContext,
            new FluentAvalonia.UI.Navigation.FrameNavigationOptions
            {
                IsNavigationStackEnabled = true,
                TransitionInfoOverride = transitionInfo ?? new SuppressNavigationTransitionInfo()
            });
    }

    public void ClearOverlay()
    {
        _overlayHost?.Children.Clear();

    }

    private Frame? _frame;
    private Panel? _overlayHost;
}

public class NavigationFactory : INavigationPageFactory
{
   
    public NavigationFactory()
    {
    }
    public Control GetPage(Type srcType)
    {
        return null;
    }

    public Control GetPageFromObject(object target)
    {
        if (target is HomePageModel)
        {
            var c = ContainerServiceHelper.Current.ContainerProvider.Resolve<IDashboardView>() as Control;
            if (c.DataContext == null)
                c.DataContext = ContainerServiceHelper.Current.ContainerProvider.Resolve<IDashboardViewModel>();
            return c;
        }
        else
        {
            return ResolvePage(target as PageBaseModel);
        }
    }

    private Control? ResolvePage(PageBaseModel pbvm)
    {
        if (pbvm is null)
            return null;

        Control page = null;
        var key = pbvm.PageKey;

        if (CorePages.TryGetValue(key, out var func))
        {
            page = func();
            //(page as ChameleonPageBase).CreationContext = pbvm;
        }
        else if (FAPages.TryGetValue(key, out func))
        {
            var pg = (ChameleonPageBase)func();
            var dc = (PageBaseModel)pbvm;

            pg.PreviewImage = ApplicationHelper.FindResource<IconSource>(dc.IconResourceKey);
            pg.ControlName = dc.Header;
            pg.Description = dc.Description;

            page = pg;
        }

        return page;
    }

    // Do this to avoid needing Activator.CreateInstance to create from type info
    // and to avoid a ridiculous amount of 'ifs'
    private readonly Dictionary<string, Func<Control>> CorePages = new Dictionary<string, Func<Control>>
    {

    };

    private readonly Dictionary<string, Func<Control>> FAPages = new Dictionary<string, Func<Control>>
    {

    };
}
