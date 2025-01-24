using System.IO;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

using Chameleon.app.Avalonia;
using Chameleon.app.Avalonia.Features.ProfilesAndFolders;
using Chameleon.app.Avalonia.Services;
using Chameleon.app.Avalonia.ViewModels;
using Chameleon.app.Avalonia.ViewModels.General;
using Chameleon.app.Avalonia.Views;
using Chameleon.app.client.ViewModels;
using Chameleon.app.client.Views;
using Chameleon.lib.Common;
using Chameleon.lib.Common.Interfaces.Services;
using Chameleon.lib.Common.Types;
using Chameleon.lib.WebBrowser.Interfaces;
using Chameleon.lib.WebBrowser.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chameleon.app.client;

public partial class App : Application {
	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);

		// Default logic doesn't auto detect windows theme anymore in designer
		// force here
		if (Design.IsDesignMode) {
			RequestedThemeVariant = ThemeVariant.Light;
		}

		IoC.Instance.Configure(() => {
			return new WritableConfiguration(new ConfigurationBuilder()
				.SetBasePath(lib.Common.Constants.Consts.AppDataDir)
				.AddJsonFile(lib.Common.Constants.Consts.AppSettingsFileName, optional: true, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build(),
				Path.Combine(lib.Common.Constants.Consts.AppDataDir, lib.Common.Constants.Consts.AppSettingsFileName));
		}, (services) => {
			_ = services
			.AddSingleton<IDispatchService, DispatchService>()
			.AddSingleton<IToasterService, ToasterService>()
			.AddSingleton<IMboxService, MboxService>()
			.AddSingleton<IShowWindowService, ShowWindowService>()
			.AddSingleton<ICopyPastaService, CopyPastaService>()
			.AddSingleton<ISysBrowserService, SysBrowserService>()
			// Main
			.AddSingleton<MainView>()
			.AddSingleton(MainViewModel.Instance)
			// Dash
			.AddSingleton<DashboardView>()
			.AddSingleton<DashboardViewModel>()
			// Projects
			.AddSingleton<ProjectsView>()
			.AddSingleton<ProjectsViewModel>()
			.AddSingleton<UserProfileIdentityView>()
			.AddSingleton<UserProfileIdentityViewModel>()
			// General
			.AddSingleton<AssistanTaskforceView>()
			.AddSingleton<AssistantTaskforceViewModel>()
			//FunctionalSettings
			.AddSingleton<FunctionalSettingsView>()
			.AddSingleton<UserProxySettingsView>()
			.AddSingleton<UserDefaultSettingsView>()
			.AddSingleton<PhoneVerificationView>()
			.AddSingleton<ProxyCreditView>()
			.AddSingleton<FunctionalSettingsViewModel>()
			.AddSingleton<UserProxySettingsViewModel>()
			.AddSingleton<UserDefaultSettingsViewModel>()
			.AddSingleton<PhoneVerificationViewModel>()
			.AddSingleton<ProxyCreditViewModel>()
			.AddSingleton<AssistantUsersViewModel>()
			//Settings
			.AddSingleton<SettingsView>()
			.AddSingleton<SettingsViewModel>()
			//Playwright
			.AddSingleton<lib.Playwright.Interfaces.ICompileScriptService, lib.Playwright.Services.CompileScriptService>()
			.AddSingleton<Chameleon.lib.Playwright.Interfaces.IPlaywriteService, Chameleon.lib.Playwright.Services.PlaywriteService>()
			.AddSingleton<Chameleon.lib.Playwright.Interfaces.IPlaywrightScriptRepository, Chameleon.lib.Playwright.Services.PlaywrightScriptRepository>()
			.AddSingleton<Chameleon.lib.Playwright.Interfaces.IChromeiumPlaywrightBrowser, Chameleon.lib.Playwright.Services.ChromeiumPlaywrightBrowser>()
			.AddSingleton<PlaywrightViewModel>()
			.AddSingleton<PlaywrightView>();

			new ProfilesAndFolderModule().ConfigureServices(services);
		});

		// Setup IoC
		IoC.Instance.Init(action: async (inited) => {
			if (inited) {
				await AppStartup.Instance.RunAsync();
			}
		});
	}

	public override void OnFrameworkInitializationCompleted()
	{
		// Line below is needed to remove Avalonia data validation.
		// Without this line you will get duplicate validations from both Avalonia and CT
		BindingPlugins.DataValidators.RemoveAt(0);

		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
			desktop.MainWindow = new MainWindow {
				DataContext = MainViewModel.Instance
			};
		} else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform) {
			singleViewPlatform.MainView = new MainView {
				DataContext = MainViewModel.Instance
			};
		}

		base.OnFrameworkInitializationCompleted();
	}
}
