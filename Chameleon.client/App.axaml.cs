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
using Chameleon.app.ViewModels;
using Chameleon.app.Views;
using Chameleon.lib;
using Chameleon.lib.Common.Interfaces.Services;
using Chameleon.lib.Interfaces.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chameleon.app;

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
			// Main
			.AddSingleton<MainView>()
			.AddSingleton(MainViewModel.Instance)
			// Dash
			.AddSingleton<DashboardView>()
			.AddSingleton<DashboardViewModel>()
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
			//Settings
			.AddSingleton<SettingsView>()
			.AddSingleton<SettingsViewModel>();

			new ProfilesAndFolderModule().ConfigureServices(services);
		});

		// Setup IoC
		IoC.Instance.Init(action: async (inited) => {
			if (inited) {
				await AppStartup.Instance.RunAsync();
				IoC.GetService<SettingsViewModel>()?.InitializSettings();
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
