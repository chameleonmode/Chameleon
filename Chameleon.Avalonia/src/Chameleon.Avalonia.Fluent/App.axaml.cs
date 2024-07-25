using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Chameleon.Auth.Services;
using Chameleon.Av.Fluent.Common.Services;
using Chameleon.Av.Fluent.ViewModels;
using Chameleon.Av.Fluent.Views;
using Chameleon.Avalonia.Controls.Automation.ViewModels;
using Chameleon.Avalonia.Controls.Automation.Views;
using Chameleon.Avalonia.Controls.Automation.Views.ViewModels;
using Chameleon.Avalonia.Controls.Dashboard;
using Chameleon.Avalonia.Controls.Dashboard.ViewModels;
using Chameleon.Avalonia.Controls.UserProfilesView;
using Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;
using Chameleon.Avalonia.Controls.UserProfileView;
using Chameleon.Avalonia.Controls.UserProfileView.Services;
using Chameleon.Avalonia.Controls.UserProfileView.ViewModels;
using Chameleon.Avalonia.Prism.Infrastructure.Extensions;
using Chameleon.Avalonia.Prism.Infrastructure.Services;
using Chameleon.Domain.Entities.Automation;
using Chameleon.Infrastructure.App.Automation;
using Chameleon.Infrastructure.Ioc;
using Chameleon.Infrastructure.Profiles;
using Chameleon.Infrastructure.Repositories;
using Chameleon.Interfaces;
using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.App.Automation.Repositories;
using Chameleon.Interfaces.App.Automation.Services;
using Chameleon.Interfaces.App.Automation.ViewModels;
using Chameleon.Interfaces.App.Automation.Views;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.App.UserProfiles.Views.List;
using Chameleon.Interfaces.Dashboard;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Modules;
using Chameleon.Interfaces.Repository;
using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.Windows;
using Chameleon.Playwright.Automation.Manager;
using Chameleon.SystemBrowser;
using DryIoc;
using Prism.DryIoc;
using System.Reflection;

namespace Chameleon.Av.Fluent;

public partial class App : PrismApplication
{
    public static Action<MainWindow> OnFramworkInitComplete;

    private MainWindow _mainWindow;
    public MainWindow MainAppWindow
    {
        get
        {
            if (_mainWindow == null)
                _mainWindow = Container.Resolve<MainWindow>();
            return _mainWindow;
        }
    }
    //public static bool FrameworkInitComplete = false;

    public override void Initialize()
    {
        //App.Current.ActualThemeVariant = ThemeVariant.Dark;
        AvaloniaXamlLoader.Load(this);

        // Default logic doesn't auto detect windows theme anymore in designer
        // to stop light mode, force here
        if (Design.IsDesignMode)
        {
            RequestedThemeVariant = ThemeVariant.Light;
        }

        // Initializes Prism.Avalonia - DO NOT REMOVE
        base.Initialize();
    }

    /// <summary>Register Services and Views.</summary>
    /// <param name="containerRegistry"></param>
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterInstance(containerRegistry);
        RegisterIocContainer(containerRegistry);

        // Services
        containerRegistry.RegisterSingleton<IHaveContainerRegistry, HasContainerRegistryService>();

        var cr = Container.Resolve<IHaveContainerRegistry>();
        cr.RegisterSingleton<IHaveContainerProvider, HasContainerProviderService>(true);
        cr.RegisterSingleton<INavigationService, NavigationService>();
        //Container.Resolve<IHaveContainerProvider>();

        cr.RegisterSingleton<Prism.Events.IEventAggregator, Prism.Events.EventAggregator>();
        cr.RegisterSingleton<ITaskDialogService, TaskDialogService>();

        containerRegistry.RegisterSingleton<IIocManager, IocManager>();

        Container
            .AddInfrastructure();
        //.AddApplication()
        //.AddModules()
        //.AddUi()
        //.RegisterTypesFrom(Assembly.GetExecutingAssembly());

        //Assemblys
        Container.RegisterTypesFrom(typeof(Chameleon.Domain.AssemblyResolver).Assembly);
        Container.RegisterTypesFrom(typeof(Chameleon.Application.AssemblyResolver).Assembly);
        Container.RegisterMapperFrom(typeof(Chameleon.Application.AssemblyResolver).Assembly);
        Container.RegisterTypesFrom(typeof(Chameleon.Avalonia.Common.AssemblyResolver).Assembly);
        Container.RegisterTypesFrom(typeof(Chameleon.Avalonia.Controls.Settings.AssemblyResolver).Assembly);
        Container.RegisterTypesFrom(typeof(AuthService).Assembly);
        Container.RegisterTypesFrom(typeof(SystemBrowserManager).Assembly);
        Container.RegisterTypesFrom(typeof(PlaywrightBrowserManager).Assembly);
        Container.RegisterTypesFrom(Assembly.GetExecutingAssembly());

        // cr.RegisterSingleton<ITaskDialogAware, MainAppSplashContent>();

        // Dialogs
        // //Chameleon.Av.Fluent.Dialogs
        Container.RegisterTypesFrom(typeof(Chameleon.Av.Fluent.Dialogs.AssemblyResolver).Assembly);
        cr.Register<IMoveUserProfilesPopupView, MoveUserProfilesPopupView>();
        cr.Register<IAddUserProfilesPopupView, AddUserProfilesPopupView>();           
        cr.Register<IUserProfileSidePanelView, UserProfileSidePanelView>();
        cr.Register<IUserProfileSidePanelViewModel, UserProfileSidePanelViewModel>();

        // Views - Viewmodels                                                     
        containerRegistry.RegisterSingleton<IMainWindow, MainWindow>();
        containerRegistry.RegisterSingleton<IMainViewViewModel, MainViewViewModel>();

        containerRegistry.RegisterSingleton<IDashboardViewModel, DashboardViewModel>();
        containerRegistry.RegisterSingleton<IDashboardView, DashboardView>();

        RegisterAutomationTypes(containerRegistry);

        cr.RegisterSingleton<IProjectsViewModel, ProjectsViewModel>();
        cr.RegisterSingleton<IProjectsView, ProjectsView>();
        cr.RegisterSingleton<IUserProfilesView, UserProfilesView>();
        cr.RegisterSingleton<IUserProfileAdditionalDataService, UserProfileAdditionalDataService>();
        Container.RegisterMapperFrom(typeof(UserProfileIdentityViewModel).Assembly);
        cr.RegisterSingleton<IUserProfileIdentityViewModel, UserProfileIdentityViewModel>();
        cr.Register<IUserProfileIdentityView, UserProfileIdentityView>();
        cr.RegisterSingleton<IUserProfileFoldersView, UserProfileFoldersView>();
        cr.RegisterSingleton<IUserProfileFoldersViewModel, UserProfileFoldersViewModel>();
        cr.RegisterSingleton<IUserProfilesViewModel, UserProfilesViewModel>();


        // Views - Region Navigation
    }

    private static void RegisterAutomationTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<IAutomationScriptRepository, AutomationScriptRepository>();
        containerRegistry.RegisterSingleton<IAutomationService, AutomationService>();
        containerRegistry.Register<IAutomationView, AutomationView>();
        containerRegistry.Register<IAutomationViewModel, AutomationViewModel>();
        containerRegistry.Register<IAutomationScriptViewModel, AutomationScriptViewModel>();
        containerRegistry.Register<IAutomationScriptParameterViewModel, AutomationScriptParameterViewModel>();
        containerRegistry.Register<IAutomationParameterValueViewModel, AutomationParameterValueViewModel>();
        containerRegistry.Register<IAutomationScriptDescription, AutomationScriptDescription>();
        containerRegistry.Register<IAddScriptParametersPopupView, AddScriptParametersPopupView>();
        containerRegistry.Register<IAddScriptParametersPopupViewModel, AddScriptParametersPopupViewModel>();
        containerRegistry.Register<ISelectAutomationPopupViewModel, SelectAutomationPopupViewModel>();
        containerRegistry.Register<ISelectAutomationPopupView, SelectAutomationPopupView>();
    }

    private void RegisterIocContainer(IContainerRegistry containerRegistry)
    {
        var container = containerRegistry.GetContainer();
        //container.AddExtension(new Diagnostic());

        //,,,,
        var factoryMethod = FactoryMethod.Of(
            typeof(StaticFactory).GetMethods().Single(m => m.Name == "Create" && m.IsGenericMethodDefinition));
        container.Register(typeof(Repository<,,,,,,>), made: Made.Of(factoryMethod));
        container.Register(typeof(Repository<,,,,>), made: Made.Of(factoryMethod));
        container.Register(typeof(Repository<,,,>), made: Made.Of(factoryMethod));
        container.Register(typeof(Repository<,,>), made: Made.Of(factoryMethod));
        container.Register(typeof(IRepository<,,>), made: Made.Of(factoryMethod));

        var factoryMethod1 = FactoryMethod.Of(
            typeof(StaticFactory).GetMethods().Single(m => m.Name == "CreateOne" && m.IsGenericMethodDefinition));
        container.Register(typeof(IRepository<>), made: Made.Of(factoryMethod1));

        //UserProfileItemRepository<,,,,> 
        var factoryMethod3 = FactoryMethod.Of(
    typeof(StaticUPFactory).GetMethods().Single(m => m.Name == "Create" && m.IsGenericMethodDefinition));
        container.Register(typeof(UserProfileItemRepository<,,,,>), made: Made.Of(factoryMethod3));

        containerRegistry.RegisterInstance(Container);
        containerRegistry.RegisterInstance(container);
    }

    /// <summary>Register optional modules in the catalog.</summary>
    /// <param name="moduleCatalog">Module Catalog.</param>
    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        //base.ConfigureModuleCatalog(moduleCatalog);
        Container
               .Resolve<IModuleLoader<IModuleCatalog>>()
               .LoadModules(moduleCatalog);
    }

    public override void OnFrameworkInitializationCompleted()
    {
       //if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
       //{
       //    desktop.MainWindow = new MainWindow();
       //}
       //else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
       //{
       //    singleView.MainView = new MainView();
       //}

        base.OnFrameworkInitializationCompleted();

        OnFramworkInitComplete?.Invoke(MainAppWindow);
        //MainAppWindow.MainView.OnFrameworkInit(MainAppWindow);
    }

    protected override AvaloniaObject CreateShell()
    {
        return MainAppWindow;
    }
}