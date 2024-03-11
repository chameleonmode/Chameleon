using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System.ComponentModel;
using System;
using System.Linq;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Mvvm;
using System.Reflection;
using System.Globalization;
using Chameleon.Core.Attributes;
using Chameleon.Interfaces.Startup;
using Microsoft.CodeAnalysis;
using Chameleon.Avalonia.Prism.Infrastructure.Extensions;
using Chameleon.Avalonia.Prism.Interfaces.Extensions;
using Chameleon.Avalonia.Prism.Domain.Extensions;
using Chameleon.Avalonia.Prism.Application.Extensions;
using Chameleon.Avalonia.PrismApp.Extensions;
using Chameleon.Interfaces.Modules;
using Chameleon.Interfaces.Repository;
using DryIoc;
using Chameleon.Infrastructure.Repositories;
using Chameleon.Infrastructure.Profiles;
using Chameleon.Avalonia.Prism.Module;
using Prism.Events;
using Chameleon.Avalonia.Prism.Module.Auth;
using Chameleon.Avalonia.Prism.Module.Auth.ViewModels;
using Chameleon.Avalonia.Prism.Module.MessageBox;
using Chameleon.Avalonia.Prism.Module.MessageBox.ViewModels;
using Chameleon.Avalonia.Prism.Infrastructure.Services;
using Chameleon.Avalonia.Controls.Dashboard.ViewModels;
using Chameleon.Avalonia.Controls.Dashboard;
using Chameleon.Avalonia.Controls.Settings.ViewModels;
using Chameleon.Avalonia.Controls.Settings;
using Chameleon.Avalonia.Controls.Sidebar;
using Chameleon.Common.Regions;
using Chameleon.Interfaces.Windows;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.Dashboard;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.DialogWindows;
using Chameleon.Avalonia.Prism.Dialogs;
using Chameleon.Interfaces.App.Settings;
using Chameleon.Avalonia.Controls.Settings.ViewModels.ProxyAccess;
using Chameleon.Avalonia.Controls.Settings.ViewModels.AssistantUsers;
using Chameleon.Controls.AssistantUsers.Interfaces;
using Chameleon.Avalonia.Controls.Settings.ViewModels.ImportExport;


namespace Chameleon.Avalonia.PrismApp;
public partial class App : PrismApplication
{
    /// <summary>App entry point.</summary>
    public App()
    {
        Console.WriteLine("Constructor()");
    }

    // Note:
    //  Though, Prism.WPF v8.1 uses, `protected virtual void Initialize()`
    //  Avalonia's AppBuilderBase.cs calls, `.Setup() { ... Instance.Initialize(); ... }`
    //  Therefore, we need this as a `public override void` in PrismApplicationBase.cs
    public override void Initialize()
    {
        Console.WriteLine("Initialize()");
        AvaloniaXamlLoader.Load(this);

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

            if (viewModelType == null)
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

        // Services
        containerRegistry.RegisterSingleton<Chameleon.Prism.Events.IEventAggregator, Chameleon.Prism.Events.EventAggregator>();

        RegisterContainerRegistry(containerRegistry);
        RegisterIocContainer(containerRegistry);

        Container
            .AddInfrastructure(containerRegistry)
            .AddDomain()
            .AddApplication()
            //.AddModules()
            .AddUi()
            ;
        //Assemblys                                                                      
        Container.RegisterTypesFrom(Chameleon.Avalonia.Common.AssemblyResolver.GetAssembly());
        Container.RegisterTypesFrom(Chameleon.Avalonia.Prism.Module.MessageBox.AssemblyResolver.GetAssembly());

        // Dialogs
        containerRegistry.RegisterDialog<AuthView, AuthViewModel>();
        containerRegistry.RegisterDialog<MessageBoxView, MessageBoxViewModel>();
        //containerRegistry.Register<object>();

        // Views - Viewmodels                                                     
        containerRegistry.RegisterSingleton<IMainWindow, MainWindow>();

        containerRegistry.RegisterSingleton<IDashboardViewModel, DashboardViewModel>();

        containerRegistry.RegisterSingleton<ISettingsViewModel, SettingsViewModel>();
        containerRegistry.RegisterSingleton<ISettingsView, SettingsView>();
        containerRegistry.RegisterSingleton<IUserDefaultSettingsViewModel, UserDefaultSettingsViewModel>();
        containerRegistry.RegisterSingleton<IUserProxySettingsViewModel, UserProxySettingsViewModel>();
        containerRegistry.RegisterSingleton<IProxyCreditViewModel, ProxyCreditViewModel>();
        containerRegistry.RegisterSingleton<IPhoneVerificationViewModel, PhoneVerificationViewModel>();
        containerRegistry.RegisterSingleton<IAssistantUsersViewModel, AssistantUsersViewModel>();
        containerRegistry.RegisterSingleton<IAssistantUsersViewModel, AssistantUsersViewModel>();
        containerRegistry.RegisterSingleton<ImportViewModel>();
        containerRegistry.Register<IBulkAddPagesPopupView, BulkAddPagesPopupView>();
        containerRegistry.Register<IProxyAccessViewModels, ProxyAccessViewModels>();

        // Views - Region Navigation
        containerRegistry.RegisterForNavigation<DashboardView, IDashboardViewModel>();
        containerRegistry.RegisterForNavigation<SettingsView, ISettingsViewModel>();
        containerRegistry.RegisterForNavigation<UserDefaultSettingsView, IUserDefaultSettingsViewModel>();
        containerRegistry.RegisterForNavigation<UserProxySettingsView, IUserProxySettingsViewModel>();
        containerRegistry.RegisterForNavigation<ProxyCreditView, IProxyCreditViewModel>();
        containerRegistry.RegisterForNavigation<PhoneVerificationView, IPhoneVerificationViewModel>();
        containerRegistry.RegisterForNavigation<AssistantUsersView, IAssistantUsersViewModel>();
        containerRegistry.RegisterForNavigation<ImportView, ImportViewModel>();
    }
    private void RegisterContainerRegistry(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterInstance(containerRegistry);
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

    /// <summary>User interface entry point, called after Register and ConfigureModules.</summary>
    /// <returns>Startup View.</returns>
    protected override AvaloniaObject CreateShell()
    {
        Console.WriteLine("CreateShell()");
        return Container.Resolve<MainWindow>();
    }

    /// <summary>Called after Initialize.</summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        // Register Views to the Region it will appear in. Don't register them in the ViewModel.
        var regionManager = Container.Resolve<IRegionManager>();

        // WARNING: Prism v11.0.0-prev4
        // - DataTemplates MUST define a DataType or else an XAML error will be thrown
        // - Error: DataTemplate inside of DataTemplates must have a DataType set                
        regionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(DashboardView));              
        regionManager.RegisterViewWithRegion(RegionNames.SidebarRegion, typeof(SidebarView));

        ////var logService = Container.Resolve<ILogService>();
        ////logService.Configure("swlog.config");
    }

    public override void OnFrameworkInitializationCompleted()
    {
        base.OnFrameworkInitializationCompleted();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime && desktopLifetime.MainWindow == null)
        {
            desktopLifetime.MainWindow = new MainWindow();
        }
                
        Container
            .Resolve<IApplicationStartup>()
            .Run();
    }
}