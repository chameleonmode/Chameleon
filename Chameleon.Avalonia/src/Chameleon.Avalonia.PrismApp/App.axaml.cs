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
            return viewModelType;
        });
    }

    /// <summary>Register Services and Views.</summary>
    /// <param name="containerRegistry"></param>
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        Console.WriteLine("RegisterTypes()");

        containerRegistry.Register<MainWindow>();

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

        // Dialogs
        containerRegistry.RegisterDialog<AuthView, AuthViewModel>();

        // Services
        //containerRegistry.RegisterSingleton<INotificationService, NotificationService>();

        // Views - Generic
        //// containerRegistry.Register<SidebarView>();  // Not required
        //// containerRegistry.Register<MainWindow>();

        // Views - Region Navigation
        //containerRegistry.RegisterForNavigation<DashboardView, DashboardViewModel>();
        //containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>();
        //containerRegistry.RegisterForNavigation<SubSettingsView, SubSettingsViewModel>();
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
        //var regionManager = Container.Resolve<IRegionManager>();

        // WARNING: Prism v11.0.0-prev4
        // - DataTemplates MUST define a DataType or else an XAML error will be thrown
        // - Error: DataTemplate inside of DataTemplates must have a DataType set
        //regionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(DashboardView));
        //regionManager.RegisterViewWithRegion(RegionNames.SidebarRegion, typeof(SidebarView));

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