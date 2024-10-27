using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

using Chameleon.app.Avalonia.Services;
using Chameleon.app.Avalonia.Views.Main;
using Chameleon.Av.Fluent.Common.Services;
using Chameleon.lib.Common;
using Chameleon.lib.Common.Interfaces.Services;
using Chameleon.lib.Common.Interfaces.Sys;
using Chameleon.lib.Common.Types;
using Chameleon.lib.WebBrowser.Interfaces;
using Chameleon.lib.WebBrowser.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using AvApplication = Avalonia.Application;

namespace Chameleon.app.Avalonia;

public partial class App : AvApplication {
	private MainWindow? _mainWindow;
	public MainWindow MainAppWindow {
		get {
			_mainWindow ??= new();
			return _mainWindow;
		}
	}
	//public static bool FrameworkInitComplete = false;

	public override void Initialize()
	{
		// Initializes Prism.Avalonia - DO NOT REMOVE
		base.Initialize();

		//App.Current.ActualThemeVariant = ThemeVariant.Dark;
		AvaloniaXamlLoader.Load(this);

		// Default logic doesn't auto detect windows theme anymore in designer
		// force here
		if (Design.IsDesignMode) {
			RequestedThemeVariant = ThemeVariant.Light;
		}


		void setup(bool init)
		{
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
			.AddSingleton<IAuthSession, ThisAuthSession>()
			//SysBrowser
			.AddSingleton<ISysBrowserService, SysBrowserService>()
			//Dash
			.AddSingleton<Chameleon.app.Avalonia.Views.DashboardView>()
			.AddSingleton<Chameleon.app.Avalonia.ViewModels.DashboardViewModel>()
			//Projects
			.AddSingleton<Chameleon.app.Avalonia.Views.ProjectsView>()
			.AddSingleton<Chameleon.app.Avalonia.ViewModels.ProjectsViewModel>()
			.AddSingleton<Chameleon.app.Avalonia.Views.UserProfileIdentityView>()
			.AddSingleton<Chameleon.app.Avalonia.ViewModels.UserProfileIdentityViewModel>()
			//FunctionalSettings
			.AddSingleton<Chameleon.app.Avalonia.Views.FunctionalSettingsView>()
			.AddSingleton<Chameleon.app.Avalonia.Views.UserProxySettingsView>()
			.AddSingleton<Chameleon.app.Avalonia.Views.UserDefaultSettingsView>()
			.AddSingleton<Chameleon.app.Avalonia.Views.PhoneVerificationView>()
			.AddSingleton<Chameleon.app.Avalonia.Views.ProxyCreditView>()
			.AddSingleton<Chameleon.app.Avalonia.Views.AssistantUsersView>()
			.AddSingleton<Chameleon.app.Avalonia.ViewModels.FunctionalSettingsViewModel>()
			.AddSingleton<Chameleon.app.Avalonia.ViewModels.UserProxySettingsViewModel>()
			.AddSingleton<Chameleon.app.Avalonia.ViewModels.UserDefaultSettingsViewModel>()
			.AddSingleton<Chameleon.app.Avalonia.ViewModels.PhoneVerificationViewModel>()
			.AddSingleton<Chameleon.app.Avalonia.ViewModels.ProxyCreditViewModel>()
			.AddSingleton<Chameleon.app.Avalonia.ViewModels.AssistantUsersViewModel>()
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
	}

	public override void OnFrameworkInitializationCompleted()
	{
		BindingPlugins.DataValidators.RemoveAt(0);
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
			desktop.MainWindow = MainAppWindow;
		} else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView) {
			singleView.MainView = new MainView();
		}

		base.OnFrameworkInitializationCompleted();
	}
}