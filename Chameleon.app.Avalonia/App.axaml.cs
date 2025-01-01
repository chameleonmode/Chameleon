using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

using Chameleon.app.Avalonia.Com.Fluent.Services;
using Chameleon.app.Avalonia.Services;
using Chameleon.lib.Common;
using Chameleon.lib.Common.Interfaces.Services;
using Chameleon.lib.Common.Types;
using Chameleon.lib.WebBrowser.Interfaces;
using Chameleon.lib.WebBrowser.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using AvApplication = Avalonia.Application;

namespace Chameleon.app.Avalonia;

public partial class App : AvApplication {
	private Views.Main.MainWindow? _mainWindow;
	public Views.Main.MainWindow MainAppWindow {
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
			.AddSingleton<ISysBrowserService, SysBrowserService>()
			// Main
			.AddSingleton<Views.Main.MainView>()
			.AddSingleton(ViewModels.AppMainViewViewModel.Instance)
			// Dash
			.AddSingleton<Views.DashboardView>()
			.AddSingleton<ViewModels.DashboardViewModel>()
			// Projects
			.AddSingleton<Views.ProjectsView>()
			.AddSingleton<ViewModels.ProjectsViewModel>()
			.AddSingleton<Views.UserProfileIdentityView>()
			.AddSingleton<ViewModels.UserProfileIdentityViewModel>()
			// General
			.AddSingleton<AssistanTaskforceView>()
			.AddSingleton<ViewModels.General.AssistantTaskforceViewModel>()
			//FunctionalSettings
			.AddSingleton<Views.FunctionalSettingsView>()
			.AddSingleton<Views.UserProxySettingsView>()
			.AddSingleton<Views.UserDefaultSettingsView>()
			.AddSingleton<Views.PhoneVerificationView>()
			.AddSingleton<Views.ProxyCreditView>()
			.AddSingleton<ViewModels.FunctionalSettingsViewModel>()
			.AddSingleton<ViewModels.UserProxySettingsViewModel>()
			.AddSingleton<ViewModels.UserDefaultSettingsViewModel>()
			.AddSingleton<ViewModels.PhoneVerificationViewModel>()
			.AddSingleton<ViewModels.ProxyCreditViewModel>()
			.AddSingleton<ViewModels.AssistantUsersViewModel>()
			//Settings
			.AddSingleton<Views.SettingsView>()
			.AddSingleton<ViewModels.SettingsViewModel>()
			//Playwright
			.AddSingleton<Chameleon.lib.Playwright.Interfaces.ICompileScriptService, Chameleon.lib.Playwright.Services.CompileScriptService>()
			.AddSingleton<Chameleon.lib.Playwright.Interfaces.IPlaywriteService, Chameleon.lib.Playwright.Services.PlaywriteService>()
			.AddSingleton<Chameleon.lib.Playwright.Interfaces.IPlaywrightScriptRepository, Chameleon.lib.Playwright.Services.PlaywrightScriptRepository>()
			.AddSingleton<Chameleon.lib.Playwright.Interfaces.IChromeiumPlaywrightBrowser, Chameleon.lib.Playwright.Services.ChromeiumPlaywrightBrowser>()
			.AddSingleton<ViewModels.PlaywrightViewModel>()
			.AddSingleton<Views.PlaywrightView>();
		});

		// Setup IoC
		IoC.Instance.Init(action: async  (inited) => {
			if (inited) {
				await AppStartup.Instance.RunAsync();
			}
		});
	}

	public override void OnFrameworkInitializationCompleted()
	{
		base.OnFrameworkInitializationCompleted();

		BindingPlugins.DataValidators.RemoveAt(0);
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
			desktop.MainWindow = MainAppWindow;
		} else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView) {
			singleView.MainView = IoC.GetService<Views.Main.MainView>();
		}
	}
}