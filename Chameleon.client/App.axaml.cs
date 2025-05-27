using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.Styling;

using Chameleon.app.Avalonia;
using Chameleon.app.Avalonia.Services;
using Chameleon.app.Avalonia.ViewModels;
using Chameleon.app.Avalonia.Views;
using Chameleon.lib;
using Chameleon.lib.Common.Interfaces.Services;
using Chameleon.lib.Interfaces.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Chameleon.client.Pages;
using Chameleon.client.Main;
using Chameleon.lib.Const;
using Chameleon.lib.WebBrowser.Services;
using Microsoft.Playwright;
using Chameleon.lib.Helpers;
using Chameleon.lib.Auth;
using Chameleon.lib.Util;
using Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity;

namespace Chameleon.client;

public partial class App : Application {
	public static IStorageProvider StorageProvider {
		get {
			return (Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow?.StorageProvider 
			?? throw new InvalidOperationException("StorageProvider not available");
		}
	}

	public static T? TryGetResource<T>(string key) where T : class {
		return Current?.TryGetResource(key, null, out var result) == true && result is T typed ? typed : default;
	}

	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);

		// Default logic doesn't auto detect windows theme anymore in designer
		// force here
		if (Design.IsDesignMode) {
			RequestedThemeVariant = ThemeVariant.Light;
		}

		// IoC.Instance.StartUps.Add(AddonsServer.Instance);

		IoC.Instance.Configure(() => {
			return new WritableConfiguration(new ConfigurationBuilder()
				.SetBasePath(FilePaths.AppDataDir)
				.AddJsonFile(Variables.AppSettingsFileName, optional: true, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build(), Path.Combine(FilePaths.AppDataDir, Variables.AppSettingsFileName));
		}, (services) => {
			_ = services
			.AddSingleton<IDispatchService, DispatchService>()
			.AddSingleton<IToasterService, ToasterService>()
			.AddSingleton<IMboxService, MboxService>()
			.AddSingleton<IShowWindowService, ShowWindowService>()
			.AddSingleton<ICopyPastaService, CopyPastaService>()
			// Main
			.AddSingleton<MainView>()
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
			.AddSingleton<SettingsViewModel>()
			.WithAllPagesAndFeatures();
		});

		// Setup IoC
		IoC.Instance.Init(action: async (inited) => {
			if (inited) {
				await AppStartup.Instance.RunAsync();
				IoC.GetService<SettingsViewModel>()?.InitializSettings();

				_ = await lib.WebBrowser.Project.Init();
				_ = await lib.Playwright.Project.Init();
			}
		});

		Navigator.Instance.RegisterView(nameof(IdentityView), typeof(IdentityView));
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
