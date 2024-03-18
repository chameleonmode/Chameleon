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
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.Windows;
using DryIoc;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using System.Globalization;
using System.Reflection;
using Chameleon.Avalonia.Prism.Infrastructure.Extensions;
using Chameleon.Avalonia.Prism.Domain.Extensions;
using Chameleon.Avalonia.Prism.Application.Extensions;
using Chameleon.Avalonia.Prism.Interfaces.Extensions;
using Chameleon.Avalonia.Controls.Dashboard.ViewModels;
using Chameleon.Avalonia.Controls.Settings.ViewModels;
using Chameleon.Avalonia.Controls.Settings;
using Chameleon.Avalonia.Controls.Settings.ViewModels.AssistantUsers;
using Chameleon.Avalonia.Controls.Settings.ViewModels.ImportExport;
using Chameleon.Avalonia.Controls.Settings.ViewModels.ProxyAccess;
using Chameleon.Avalonia.Controls.Dashboard;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Av.Fluent.Common.Services;
using Chameleon.Av.Fluent.Dialogs;
using Chameleon.Avalonia.Prism.Module.MessageBox;
using Chameleon.Av.Fluent.Dialogs.ViewModels;
using Chameleon.Interfaces.Dialogs.Views;
using Chameleon.Av.Fluent.Dialogs.Controls;
using Chameleon.Av.Fluent.Dialogs.Services;

namespace Chameleon.Av.Fluent;

public partial class App : PrismApplication
{
    public static bool FrameworkInitComplete = false;

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

        ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
        {
            var attr = viewType.GetCustomAttribute<ViewModelAttribute>();
            if (attr != null)
            {
                return attr.Type;
            }

            var viewName = viewType.FullName;
            viewName = viewName.Replace(".Views.", ".ViewModels.");
            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            var suffix = viewName.EndsWith("View") ? "Model" : "ViewModel";
            var viewModelName = String.Format(CultureInfo.InvariantCulture, "{0}{1}, {2}", viewName, suffix, viewAssemblyName);
            var viewModelType = Type.GetType(viewModelName);

            if (viewModelType == null && viewType.Name != "MainWindow")
            {
                viewModelType = Type.GetType($"{viewType.FullName}Model");
            }
            return viewModelType;
        });
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
        //Container.Resolve<IHaveContainerProvider>();

        cr.RegisterSingleton<Prism.Events.IEventAggregator, Prism.Events.EventAggregator>();
        cr.RegisterSingleton<ITaskDialogService, TaskDialogService>();


        //AddInfrastructure
        //containerRegistry.RegisterSingleton<IHaveContainerRegistry, HasContainerRegistryService>();
        //containerRegistry.RegisterSingleton<IHaveContainerProvider, HasContainerProviderService>();

        //containerRegistry.RegisterSingleton<IIocManager, IocManager>();

        Container
            .AddInfrastructure(containerRegistry)
            .AddDomain()
            .AddApplication()
            //.AddModules()
            //.AddUi()
            .RegisterTypesFrom(Assembly.GetExecutingAssembly());
        //Assemblys                                                                      
        Container.RegisterTypesFrom(Chameleon.Avalonia.Common.AssemblyResolver.GetAssembly());
        Container.RegisterTypesFrom(Chameleon.Avalonia.Prism.Module.MessageBox.AssemblyResolver.GetAssembly());   

       // cr.RegisterSingleton<ITaskDialogAware, MainAppSplashContent>();

        // Dialogs                                                  
        //containerRegistry.RegisterDialog<AuthView, AuthViewModel>(nameof(IAuthLoginView));
        //containerRegistry.RegisterDialogWindow<DialogWindowsWindow>(nameof(IWindowWindowDialog));  
        cr.RegisterSingleton<IContentDialogService, ContentDialogService>();
        //cr.RegisterSingleton<ILoginTaskDialog, LoginTaskDialog>(false, Chameleon.Common.Regions.DialogNames.LoginDialog);
        cr.Register<ILoginContentDialogContent, LoginContentDialogContent>();
        cr.Register<DefaultContentDialogTitle>();
        cr.RegisterSingleton<IAuthTaskDialogViewModel, AuthTaskDialogViewModel>();
        //cr.RegisterSingleton<ILoginTaskDialog, LoginTaskDialog>(false, Chameleon.Common.Regions.DialogNames.LoginDialog);
        //containerRegistry.RegisterDialog<MessageBoxView, MessageBoxViewModel>();
        //containerRegistry.Register<object>();

        // Views - Viewmodels                                                     
        containerRegistry.RegisterSingleton<IMainWindow, MainWindow>();

        containerRegistry.RegisterSingleton<IDashboardViewModel, DashboardViewModel>();
        containerRegistry.RegisterSingleton<IDashboardView, DashboardView>();

        containerRegistry.RegisterSingleton<ISettingsViewModel, SettingsViewModel>();
        containerRegistry.RegisterSingleton<ISettingsView, SettingsView>();
        containerRegistry.RegisterSingleton<IUserDefaultSettingsViewModel, UserDefaultSettingsViewModel>();
        containerRegistry.RegisterSingleton<IUserProxySettingsViewModel, UserProxySettingsViewModel>();
        containerRegistry.RegisterSingleton<IProxyCreditViewModel, ProxyCreditViewModel>();
        containerRegistry.RegisterSingleton<IPhoneVerificationViewModel, PhoneVerificationViewModel>();
        containerRegistry.RegisterSingleton<IAssistantUsersViewModel, AssistantUsersViewModel>();
        containerRegistry.RegisterSingleton<ImportViewModel>();
        containerRegistry.Register<IBulkAddPagesPopupView, BulkAddPagesPopupView>();
        containerRegistry.Register<IProxyAccessViewModels, ProxyAccessViewModels>();
                                                                                        
        
        // Views - Region Navigation
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

        FrameworkInitComplete = true;
    }

    protected override AvaloniaObject CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }
}