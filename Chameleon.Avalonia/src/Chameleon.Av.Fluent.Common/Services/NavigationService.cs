using Avalonia.Controls;
using Chameleon.Av.Fluent.Common.Models;
using Chameleon.Av.Fluent.Common.Pages;
using Chameleon.Avalonia.Common.Helpers;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.Dashboard;
using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Settings;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Controls.Primitives;
using FluentAvalonia.UI.Media.Animation;
using FluentAvalonia.UI.Navigation;
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
    public void NavigateToType(Type t,object? parameter = null, NavigationTransitionInfo? transitionInfo = null)
    {
        _frame?.NavigateToType(t,  parameter, BuildOptions(transitionInfo));
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
}

public class NavigationFactory : INavigationPageFactory
{
   
    public NavigationFactory()
    {
    }
    public Control GetPage(Type srcType)
    {
        var c = ContainerServiceHelper.Resolve(srcType) as Control;
        return c;
    }

    public Control? GetPageFromObject(object target)
    {
        if (target is HomePageModel)
        {
            var c = ContainerServiceHelper.Resolve<IDashboardView>() as Control;
            return c;
        }
        else if (target is SettingsPageModel)
        {
            var c = ContainerServiceHelper.Resolve<ISettingsView>() as Control;
            return c;
        }
        else if (target is string nameOf)
        {
            var c = ContainerServiceHelper.Resolve<ISettingsView>() as Control;
            return ResolvePage(nameOf);
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
    private Control? ResolvePage(string pbvm)
    {
        Control? page = null;

        if (SettingsPages.TryGetValue(pbvm, out var func))
        {
            page = func();
            //(page as ChameleonPageBase).CreationContext = pbvm;
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

    private readonly Dictionary<string, Func<Control?>> SettingsPages = new()
    {
         { nameof(IUserDefaultSettingsView), () =>  ContainerServiceHelper.Resolve<IUserDefaultSettingsView>() as Control },
    };
}
