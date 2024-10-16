using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

using Chameleon.app.Avalonia.Services;
using Chameleon.Av.Fluent.Common.Services;
using Chameleon.Av.Fluent.Views;
using Chameleon.Avalonia.Prism.Infrastructure.Extensions;
using Chameleon.Avalonia.Prism.Infrastructure.Services;
using Chameleon.Infrastructure.Ioc;
using Chameleon.Infrastructure.Profiles;
using Chameleon.Infrastructure.Repositories;
using Chameleon.Interfaces.App.UserProfiles.Views.List;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Modules;
using Chameleon.Interfaces.Repository;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.lib.Common;
using Chameleon.lib.Common.Interfaces.Services;
using Chameleon.lib.Common.Types;
using Chameleon.lib.Playwright.Interfaces;
using Chameleon.lib.WebBrowser.Interfaces;
using Chameleon.lib.WebBrowser.Services;

using DryIoc;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Prism.DryIoc;
using Prism.Ioc;

using System.Reflection;

namespace Chameleon.Av.Fluent;

public class AuthSession : IAuthSession {
	public long UserId { get; set; }
	public long? CreatorUserId { get; set; }
	public string UserName { get; set; }
	public string AuthToken { get; set; }
	public bool HasAuthToken => !string.IsNullOrEmpty(AuthToken);
	public long ExpireInSeconds { get; set; }
	public string EncryptedAccessToken { get; set; }
	public string AuthRefreshToken { get; set; }
	public string[] Permissions { get; set; }
	public ILimits Limits { get; set; }
	public bool TookGuidedTour { get; set; }
	public bool CanCreateProfiles { get; set; }
}
public partial class App : PrismApplication {
	public static Action<MainWindow>? OnFramworkInitComplete;

	private MainWindow? _mainWindow;
	public MainWindow MainAppWindow {
		get {
			_mainWindow ??= Container.Resolve<MainWindow>();
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
		if (Design.IsDesignMode) {
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

		//cr.RegisterSingleton<lib.Common.Interfaces.Sys.IEventAggregator, lib.Common.Interfaces.Sys.EventAggregator>();
		//cr.RegisterSingleton<ITaskDialogService, TaskDialogService>();

		containerRegistry.RegisterSingleton<IIocManager, IocManager>();
		//containerRegistry.RegisterSingleton<IExtensionLoaderService, ExtensionLoaderService>();

		Container.AddInfrastructure();

		//Assemblys
		Container.RegisterTypesFrom(typeof(Chameleon.Domain.AssemblyResolver).Assembly);
		Container.RegisterTypesFrom(typeof(Chameleon.Application.AssemblyResolver).Assembly);
		Container.RegisterMapperFrom(typeof(Chameleon.Application.AssemblyResolver).Assembly);
		Container.RegisterTypesFrom(typeof(Chameleon.Avalonia.Common.AssemblyResolver).Assembly);
		Container.RegisterTypesFrom(Assembly.GetExecutingAssembly());

		// cr.RegisterSingleton<ITaskDialogAware, MainAppSplashContent>();

		// Dialogs
		// //Chameleon.Av.Fluent.Dialogs
		//cr.Register<IMoveUserProfilesPopupView, MoveUserProfilesPopupView>();
		//cr.Register<IAddUserProfilesPopupView, AddUserProfilesPopupView>();
		//cr.Register<IUserProfileSidePanelView, UserProfileSidePanelView>();
		//cr.Register<IUserProfileSidePanelViewModel, UserProfileSidePanelViewModel>();

		// Views - Viewmodels
		//Container.RegisterTypesFrom(typeof(Chameleon.Avalonia.Controls.Settings.Functional.ViewModels.FunctionalSettingsViewModel).Assembly);
		//Container.RegisterTypesFrom(typeof(Chameleon.Avalonia.Controls.UserProfilesView.ViewModels.ProjectsViewModel).Assembly);
		Container.RegisterTypesFrom(typeof(Chameleon.Av.Fluent.ViewModels.MainViewViewModel).Assembly);
		//Container.RegisterTypesFrom(typeof(Chameleon.Avalonia.Controls.Dashboard.ViewModels.DashboardViewModel).Assembly);
		//Container.RegisterTypesFrom(typeof(Chameleon.Avalonia.Controls.UserProfileView.ViewModels.UserProfileIdentityViewModel).Assembly);
		//Container.RegisterMapperFrom(typeof(Chameleon.Avalonia.Controls.UserProfilesView.ViewModels.ProjectsViewModel).Assembly);
	}

	private void RegisterIocContainer(IContainerRegistry containerRegistry)
	{
		var container = containerRegistry.GetContainer();
		//container.AddExtension(new Diagnostic());
		// Register logging services
		//var serviceCollection = new ServiceCollection();
		//serviceCollection.AddLogging(configure => configure.AddConsole());
		//var serviceProvider = serviceCollection.BuildServiceProvider();
		//containerRegistry.RegisterInstance(serviceProvider.GetService<ILoggerFactory>());
		//containerRegistry.Register(typeof(ILogger<>), typeof(Logger<>));

		void setup(bool init)
		{
			containerRegistry.RegisterInstance(IoC.GetService<ILoggerFactory>());
			containerRegistry.Register(typeof(ILogger<>), typeof(Logger<>));

			containerRegistry.RegisterInstance(IoC.GetService<IPlaywriteService>());
			containerRegistry.RegisterInstance(IoC.GetService<IPlaywrightScriptRepository>());
			containerRegistry.RegisterInstance(IoC.GetService<ISysBrowserService>());
			containerRegistry.RegisterInstance(IoC.GetService<ICopyPastaService>());
			containerRegistry.RegisterInstance(IoC.GetService<INavigationService>());
			containerRegistry.RegisterInstance(IoC.GetService<Chameleon.lib.Common.Interfaces.Sys.IEventAggregator>());

			containerRegistry.RegisterInstance(IoC.GetService<Chameleon.app.Avalonia.Views.PlaywrightView>());
			containerRegistry.RegisterInstance(IoC.GetService<Chameleon.app.Avalonia.Views.SettingsView>());
		}

		IoC.Instance.Configure(() => {
			return new WritableConfiguration(new ConfigurationBuilder()
				.SetBasePath(Chameleon.lib.Common.Constants.Consts.AppDataDir)
				.AddJsonFile(Chameleon.lib.Common.Constants.Consts.AppSettingsFileName, optional: true, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build(), 
				Path.Combine(Chameleon.lib.Common.Constants.Consts.AppDataDir, Chameleon.lib.Common.Constants.Consts.AppSettingsFileName));
		}, (services) => {
			_ = services
			.AddSingleton<IDispatchService, DispatchService>()
			.AddSingleton<IToasterService, ToasterService>()
			.AddSingleton<IMboxService, MboxService>()
			.AddSingleton<IShowWindowService, ShowWindowService>()
			.AddSingleton<ICopyPastaService, CopyPastaService>()
			.AddSingleton<INavigationService, NavigationService>()
			.AddSingleton<Chameleon.lib.Common.Interfaces.Sys.IEventAggregator, Chameleon.lib.Common.Interfaces.Sys.EventAggregator>()
			//SysBrowser
			.AddSingleton<ISysBrowserService, SysBrowserService>()
			//Dash
			.AddSingleton<Chameleon.app.Avalonia.Views.DashboardView>()
			.AddSingleton<Chameleon.app.Avalonia.ViewModels.DashboardViewModel>()
			//Settings
			.AddSingleton<Chameleon.app.Avalonia.Views.SettingsView>()
			.AddSingleton<Chameleon.app.Avalonia.ViewModels.SettingsViewModel>()
			//Playwright
			.AddSingleton<Chameleon.lib.Playwright.Interfaces.ICompileScriptService, Chameleon.lib.Playwright.Services.CompileScriptService>()
			.AddSingleton<Chameleon.lib.Playwright.Interfaces.IPlaywriteService, Chameleon.lib.Playwright.Services.PlaywriteService>()
			.AddSingleton<Chameleon.lib.Playwright.Interfaces.IPlaywrightScriptRepository, Chameleon.lib.Playwright.Services.PlaywrightScriptRepository>()
			.AddSingleton<Chameleon.lib.Playwright.Interfaces.IChromeiumPlaywrightBrowser, Chameleon.lib.Playwright.Services.ChromeiumPlaywrightBrowser>()
			.AddSingleton<Chameleon.app.Avalonia.ViewModels.PlaywrightViewModel>()
			.AddSingleton<Chameleon.app.Avalonia.Views.PlaywrightView>();
		});
		// Setup IoC
		IoC.Instance.Init(action: setup);

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
		BindingPlugins.DataValidators.RemoveAt(0);
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