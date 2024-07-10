using Avalonia.Controls;
using Chameleon.Av.Fluent.Common.Models;
using Chameleon.Av.Fluent.Common.Pages;
using Chameleon.Avalonia.Common.Helpers;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.Dashboard;
using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.UserProfiles;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Controls.Primitives;
using FluentAvalonia.UI.Media.Animation;
using FluentAvalonia.UI.Navigation;
using System.Configuration;

namespace Chameleon.Av.Fluent.Common.Services;

public class NavigationService : INavigationService
{
    public NavigationFactory NavigationFactory { get; }

    public NavigationService()
    {
        NavigationFactory = new NavigationFactory();
    }

    public static NavigationService Instance { get; } = ContainerServiceHelper.Resolve<INavigationService>() as NavigationService;

    public object? PreviousPage { get; set; }
    public object? NavFactory => throw new NotImplementedException();//{ get; } = new NavigationFactory();

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

    public Task InitializeAsync()
    {
        throw new NotImplementedException();
    }

    public Task NavigateToAsync(string route, IDictionary<string, object> routeParameters = null)
    {
        throw new NotImplementedException();
    }

    public Task NavigateToAsync(Type viewModel)
    {
        throw new NotImplementedException();
    }

    public Task PopAsync()
    {
        //TODO: implement other back possibilitys when they come up
        if (_frame?.CanGoBack == true && _frame.Content.GetType().Name == "UserProfileIdentityView")
            _frame?.GoBack();

        return Task.CompletedTask;
    }
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
        if (target is MainPageModelBase t)
        {
            Control? c = null;

            if (t.NavHeader == "Dashboard")
                c = ContainerServiceHelper.Resolve<IDashboardView>() as Control;
            else if (t.NavHeader == "Profiles")
                c = ContainerServiceHelper.Resolve<IProjectsView>() as Control;
            else if (t.NavHeader == "Automation")
                c = ContainerServiceHelper.Resolve<IProjectsView>() as Control;
            else if (t.NavHeader == "Settings")
                c = ContainerServiceHelper.Resolve<ISettingsView>() as Control;

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
    private readonly Dictionary<string, Func<Control>> CorePages = [];

    private readonly Dictionary<string, Func<Control>> FAPages = [];

    private readonly Dictionary<string, Func<Control?>> SettingsPages = new()
    {
         { nameof(IUserDefaultSettingsView), () =>  ContainerServiceHelper.Resolve<IUserDefaultSettingsView>() as Control },
    };
}
