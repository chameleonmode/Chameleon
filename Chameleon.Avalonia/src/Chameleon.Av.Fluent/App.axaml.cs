using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Chameleon.Av.Fluent.Views;
using Chameleon.Avalonia.Prism.Infrastructure.Services;
using Chameleon.Controls.AssistantUsers.Interfaces;
using Chameleon.Core.Attributes;
using Chameleon.Infrastructure.Ioc;
using Chameleon.Infrastructure.Profiles;
using Chameleon.Infrastructure.Repositories;
using Chameleon.Interfaces.App.Settings;
using Chameleon.Interfaces.Dashboard;
using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Modules;
using Chameleon.Interfaces.Repository;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.Startup;
using Chameleon.Interfaces.Windows;
using DryIoc;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;

using System.Globalization;
using System.Reflection;
using Chameleon.Avalonia.Prism.Infrastructure.Extensions;
using Chameleon.Avalonia.Controls.Dashboard.ViewModels;
using Chameleon.Avalonia.Controls.Settings.ViewModels;
using Chameleon.Avalonia.Controls.Settings;
using Chameleon.Avalonia.Controls.Settings.ViewModels.AssistantUsers;
using Chameleon.Avalonia.Controls.Settings.ViewModels.ProxyAccess;
using Chameleon.Avalonia.Controls.Dashboard;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Av.Fluent.Common.Services;
using Chameleon.Av.Fluent.Dialogs;
using Chameleon.Av.Fluent.Dialogs.ViewModels;
using Chameleon.Interfaces.Dialogs.Views;
using Chameleon.Av.Fluent.Dialogs.Controls;
using Chameleon.Av.Fluent.Dialogs.Services;
using Chameleon.Interfaces.Dialogs.ViewModels;
using Chameleon.Av.Fluent.ViewModels;
using Chameleon.Interfaces.DialogWindows;
using Chameleon.Interfaces.Views;
using System.ComponentModel;
using Avalonia.Svg.Skia;
using Chameleon.Interfaces.Services;
using FluentAvalonia.UI.Windowing;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Avalonia.Controls.UserProfilesView;
using Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Avalonia.Controls.UserProfileView.ViewModels;
using Chameleon.Avalonia.Controls.UserProfileView.Services;
using Chameleon.Avalonia.Controls.UserProfileView;
using Chameleon.Interfaces.App.UserProfiles.Views.List;

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
        Container.RegisterTypesFrom(typeof(Chameleon.Avalonia.Common.AssemblyResolver).Assembly );  
        Container.RegisterTypesFrom(typeof(Chameleon.Avalonia.Controls.Settings.AssemblyResolver).Assembly);     
        Container.RegisterTypesFrom(Assembly.GetExecutingAssembly());

        // cr.RegisterSingleton<ITaskDialogAware, MainAppSplashContent>();

        // Dialogs
        // //Chameleon.Av.Fluent.Dialogs
        Container.RegisterTypesFrom(Chameleon.Av.Fluent.Dialogs.AssemblyResolver.GetAssembly());
        //cr.RegisterSingleton<IContentDialogService, ContentDialogService>();
        cr.Register<ILoginContentDialogContent, LoginContentDialogContent>();
        cr.Register<IMoveUserProfilesPopupView, MoveUserProfilesPopupView>();
        cr.Register<IAddUserProfilesPopupView, AddUserProfilesPopupView>();     //
        //cr.RegisterSingleton<IDefaultContentDialogTitle, DefaultContentDialogTitle>();
        //cr.RegisterSingleton<IAuthTaskDialogViewModel, AuthTaskDialogViewModel>();
        //cr.RegisterSingleton<IBulkAddPagesPopupView, BulkAddPagesPopupView>();
        //cr.RegisterSingleton<IBulkAddPagesPopupViewModel, BulkAddPagesPopupViewModel>();

        // Views - Viewmodels                                                     
        containerRegistry.RegisterSingleton<IMainWindow, MainWindow>();

        containerRegistry.RegisterSingleton<MainViewViewModel>();

        containerRegistry.RegisterSingleton<IDashboardViewModel, DashboardViewModel>();
        containerRegistry.RegisterSingleton<IDashboardView, DashboardView>();

        containerRegistry.RegisterSingleton<IProjectsViewModel, ProjectsViewModel>();
        containerRegistry.RegisterSingleton<IProjectsView, ProjectsView>();
        containerRegistry.RegisterSingleton<IUserProfileFoldersViewModel, UserProfileFoldersViewModel>();
        containerRegistry.RegisterSingleton<IUserProfileFoldersView, UserProfileFoldersView>();
        containerRegistry.RegisterSingleton<IUserProfilesViewModel, UserProfilesViewModel>();
        containerRegistry.RegisterSingleton<IUserProfilesView, UserProfilesView>();                         
        containerRegistry.RegisterSingleton<IUserProfileAdditionalDataService, UserProfileAdditionalDataService>();
        Container.RegisterMapperFrom(typeof(UserProfileIdentityViewModel).Assembly);
        containerRegistry.RegisterSingleton<IUserProfileIdentityViewModel, UserProfileIdentityViewModel>(); 
        containerRegistry.Register<IUserProfileIdentityView, UserProfileIdentityView>();
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