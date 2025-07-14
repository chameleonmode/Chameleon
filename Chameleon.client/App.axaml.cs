using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Chameleon.lib;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Chameleon.lib.Util;
using Chameleon.client.Features.Projects.Profiles.Identity;
using Chameleon.client.Features;
using Chameleon.lib.Helpers;
using Chameleon.lib.Auth;
using Chameleon.client.Services;
using Chameleon.client.UI.Components;
using FluentAvalonia.UI.Windowing;
using Chameleon.client.Features.Settings.Featured;
using Chameleon.client.UI.Components.ViewModels;
using Chameleon.lib.Services;

namespace Chameleon.client;

public partial class App : Application {
	public static bool DEBUGGING { get; set; } = false;
	public static Window? MainWindow => (Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

	public static T? TryGetResource<T>(string key) where T : class {
		return Current?.TryGetResource(key, null, out var result) == true && result is T typed ? typed : default;
	}

	public override void Initialize() {
		AvaloniaXamlLoader.Load(this);

		// Default logic doesn't auto detect windows theme anymore in designer
		// force here
		if (Design.IsDesignMode) {
			RequestedThemeVariant = ThemeVariant.Light;
		}

		// IoC.Instance.StartUps.Add(AddonsServer.Instance);
		IoC.I.Configure((b) => {
			_ = b.SetBasePath(FilePaths.AppDataDir);
		}, (services) => {
			_ = services
			.AddSingleton<IDispatchService, DispatchService>()
			.AddSingleton<IToasterService, ToasterService>()
			.AddSingleton<IMboxService, MboxService>()
			.AddSingleton<IShowWindowService, ShowWindowService>()
			.AddSingleton<ICopyPastaService, CopyPastaService>()
			.All();
		});

		// Setup IoC
		IoC.I.Init(action: async (inited) => {
			if (inited) {
				Toaster.Info("Starting...");
				await RunAsync();
				IoC.GetService<Features.Settings.ViewModel>()?.InitializSettings();

				_ = await lib.WebBrowser.Project.Init();
				_ = await lib.Playwright.Project.Init();
			}
		});

		Navigator.Instance.RegisterView("Features.Projects.View", typeof(Features.Projects.View));
		Navigator.Instance.RegisterView(nameof(IdentityView), typeof(IdentityView));
		Navigator.Instance.RegisterView(nameof(FunctionalSettingsView), typeof(FunctionalSettingsView));
	}

	public override void OnFrameworkInitializationCompleted() {
		base.OnFrameworkInitializationCompleted();
		// Line below is needed to remove Avalonia data validation.
		// Without this line you will get duplicate validations from both Avalonia and CT
		BindingPlugins.DataValidators.RemoveAt(0);

		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
			var window = new UI.Windows.Main {
				DataContext = ViewModel.Instance,
				SplashScreen = new SplashScreen(),
				// ExtendClientAreaToDecorationsHint = true;
				// ExtendClientAreaChromeHints = Avalonia.Platform.ExtendClientAreaChromeHints.Default;
			};
			window.TitleBar.ExtendsContentIntoTitleBar = true;
			window.TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
#if DEBUG
			DEBUGGING = true;
			window.AttachDevTools();
			window.Topmost = true;
#endif
			desktop.MainWindow = window;
		} else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform) {
			singleViewPlatform.MainView = new View {
				DataContext = ViewModel.Instance
			};
		}
	}

	static async Task RunAsync(int trys = 3) {
		await EX.Try(async () => {
			if (trys == 0) throw new Exception("Failed after 3 attempts");
			else if (await EX.Catch(
				async () => await Common.Project.Logineer(
					Session.I.Settings.AutoLogin ? Session.I.Settings : (
						await MessageBox.Show<MboxLoginUserControl, MboxLoginViewModel>(
						new(Session.I.Settings), new("User Login", Symbas: Symbas.ContactInfo))
					)!.Settings
				), async e => await RunAsync(trys - 1))
			) await ViewModel.Instance.Init();
		}, async e => {
			if (await MessageBox.Error("Login Error", "Try again with last saved login info?", e)) await RunAsync();
			else {
				await Session.I.Logout();
				await RunAsync();
			}
		});
	}
}
