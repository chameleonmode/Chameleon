using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Chameleon.Auth.Services;
using Chameleon.Av.Fluent.Common.Services;
using Chameleon.Av.Fluent.Dialogs;
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
using Chameleon.Avalonia.Playwright.Automation.Chrome;
using Chameleon.Avalonia.Playwright.Automation.Manager;
using Chameleon.Avalonia.Prism.Infrastructure.Extensions;
using Chameleon.Avalonia.Prism.Infrastructure.Services;
using Chameleon.Domain.Entities.Automation;
using Chameleon.Infrastructure.App.Automation;
using Chameleon.Infrastructure.Ioc;
using Chameleon.Infrastructure.Profiles;
using Chameleon.Infrastructure.Repositories;
using Chameleon.Interfaces;
using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.App.Automation.Manager;
using Chameleon.Interfaces.App.Automation.Repositories;
using Chameleon.Interfaces.App.Automation.Services;
using Chameleon.Interfaces.App.Automation.ViewModels;
using Chameleon.Interfaces.App.Automation.Views;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.App.UserProfiles.Views.List;
using Chameleon.Interfaces.Dashboard;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Dialogs.Views;
using Chameleon.Interfaces.DialogWindows;
using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Modules;
using Chameleon.Interfaces.Repository;
using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.Views;
using Chameleon.Interfaces.Windows;
using Chameleon.SystemBrowser;
using DryIoc;
using Prism.DryIoc;
using System.ComponentModel;
using System.Reflection;

namespace Chameleon.Av.Fluent;
public class tempinits : IDialogWindowsService  , IPopupDialogService
{
    public Task<int> ShowDialogWindow(IViewControl viewControl, string title)
    {
        throw new NotImplementedException();
    }

    public Task<int> ShowDialogWindow<TViewModel>(IViewControl viewControl, string title, Action<TViewModel> initialize) where TViewModel : class
    {
        throw new NotImplementedException();
    }

    public Task<IPopupDialogResult?> Create<T>() where T : INotifyPropertyChanged
    {
        throw new NotImplementedException();
    }

    public IDialog Create(Type dialogType)
    {
        throw new NotImplementedException();
    }

    public void ShowDialog(string name, string message, Action<int?> result)
    {
        throw new NotImplementedException();
    }

    public void Close(object? result = null)
    {
        throw new NotImplementedException();
    }

    public Task CloseAsync(object? result = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void ShowDialogInWindow<TDialog, TWindow>(string message, Action<int?> result)
    {
        throw new NotImplementedException();
    }
}
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
            RequestedThemeVariant = ThemeVariant.Dark;
        }

        // Initializes Prism.Avalonia - DO NOT REMOVE
        base.Initialize();
    }

    protected override void ConfigureViewModelLocator()
    {
        base.ConfigureViewModelLocator();

        //ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
        //{
        //    var attr = viewType.GetCustomAttribute<ViewModelAttribute>();
        //    if (attr != null)
        //    {
        //        return attr.Type;
        //    }

        //    var viewName = viewType.FullName;
        //    viewName = viewName.Replace(".Views.", ".ViewModels.");
        //    var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
        //    var suffix = viewName.EndsWith("View") ? "Model" : "ViewModel";
        //    var viewModelName = String.Format(CultureInfo.InvariantCulture, "{0}{1}, {2}", viewName, suffix, viewAssemblyName);
        //    var viewModelType = Type.GetType(viewModelName);

        //    if (viewModelType == null && viewType.Name != "MainWindow")
        //    {
        //        viewModelType = Type.GetType($"{viewType.FullName}Model");
        //    }
        //    return viewModelType;
        //});
    }

    /// <summary>Register Services and Views.</summary>
    /// <param name="containerRegistry"></param>
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        Console.WriteLine("RegisterTypes()");

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
        cr.RegisterSingleton<IDialogWindowsService, tempinits>();

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
        Container.RegisterTypesFrom(Assembly.GetExecutingAssembly());

        // cr.RegisterSingleton<ITaskDialogAware, MainAppSplashContent>();

        // Dialogs
        // //Chameleon.Av.Fluent.Dialogs
        Container.RegisterTypesFrom(Chameleon.Av.Fluent.Dialogs.AssemblyResolver.GetAssembly());
        //cr.RegisterSingleton<IContentDialogService, ContentDialogService>();
        cr.Register<ILoginContentDialogContent, LoginContentDialogContent>();
        cr.Register<IMoveUserProfilesPopupView, MoveUserProfilesPopupView>();
        cr.Register<IAddUserProfilesPopupView, AddUserProfilesPopupView>();     //      
        cr.Register<IUserProfileSidePanelView, UserProfileSidePanelView>();
        cr.Register<IUserProfileSidePanelViewModel, UserProfileSidePanelViewModel>();
        //cr.RegisterSingleton<IDefaultContentDialogTitle, DefaultContentDialogTitle>();
        //cr.RegisterSingleton<IAuthTaskDialogViewModel, AuthTaskDialogViewModel>();
        //cr.RegisterSingleton<IBulkAddPagesPopupView, BulkAddPagesPopupView>();
        //cr.RegisterSingleton<IBulkAddPagesPopupViewModel, BulkAddPagesPopupViewModel>();

        // Views - Viewmodels                                                     
        containerRegistry.RegisterSingleton<IMainWindow, MainWindow>();

        containerRegistry.RegisterSingleton<IMainViewViewModel, MainViewViewModel>();

        containerRegistry.RegisterSingleton<IDashboardViewModel, DashboardViewModel>();
        containerRegistry.RegisterSingleton<IDashboardView, DashboardView>();

        RegisterAutomationTypes(containerRegistry);
        RegisterPlaywrightTypes(containerRegistry);

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
        //containerRegistry.RegisterSingleton<ISettingsViewModel, SettingsViewModel>();
        //containerRegistry.RegisterSingleton<ISettingsView, SettingsView>();
        //containerRegistry.RegisterSingleton<IUserProxySettingsViewModel, UserProxySettingsViewModel>();
        //containerRegistry.RegisterSingleton<IProxyCreditViewModel, ProxyCreditViewModel>();
        //containerRegistry.RegisterSingleton<IPhoneVerificationViewModel, PhoneVerificationViewModel>();
        //containerRegistry.RegisterSingleton<IAssistantUsersViewModel, AssistantUsersViewModel>();
        //containerRegistry.RegisterSingleton<ImportViewModel>();
        //containerRegistry.RegisterSingleton<IProxyAccessViewModels, ProxyAccessViewModels>();


        // Views - Region Navigation 
        //containerRegistry.RegisterSingleton<IUserDefaultSettingsViewModel, UserDefaultSettingsViewModel>();
        //containerRegistry.RegisterSingleton<IUserDefaultSettingsView, UserDefaultSettingsView>();
        //containerRegistry.RegisterForNavigation<DashboardView, IDashboardViewModel>();
        //containerRegistry.RegisterForNavigation<SettingsView, ISettingsViewModel>();
        //containerRegistry.RegisterForNavigation<UserDefaultSettingsView, IUserDefaultSettingsViewModel>();
        //containerRegistry.RegisterForNavigation<UserProxySettingsView, IUserProxySettingsViewModel>();
        //containerRegistry.RegisterForNavigation<ProxyCreditView, IProxyCreditViewModel>();
        //containerRegistry.RegisterForNavigation<PhoneVerificationView, IPhoneVerificationViewModel>();
        //containerRegistry.RegisterForNavigation<AssistantUsersView, IAssistantUsersViewModel>();
        //containerRegistry.RegisterForNavigation<ImportView, ImportViewModel>();
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
    }

    private void RegisterPlaywrightTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<IPlaywrightBrowserManager, PlaywrightBrowserManager>();
        containerRegistry.RegisterSingleton<IChromePlaywrightBrowser, ChromePlaywrightBrowser>();
        //containerRegistry.RegisterSingleton<IBravePlaywrightBrowser, BravePlaywrightBrowser>();
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