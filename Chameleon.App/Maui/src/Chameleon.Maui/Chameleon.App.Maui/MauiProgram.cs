using Chameleon.Interfaces.Services;
using Chameleon.Maui.Pages.Login.ViewModels;
using Chameleon.Maui.Pages.Login.Views;
using Chameleon.Maui.Pages.Settings.ViewModels;
using Chameleon.Maui.Pages.Settings.Views;
using Chameleon.Maui.Toolkit.Base;
using Chameleon.Maui.Toolkit.Helpers;
using Chameleon.Maui.Toolkit.Services;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.ApplicationModel;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Storage;
using Microsoft.Extensions.Logging;
using Chameleon.Infrastructure.Environments;
using Chameleon.Interfaces.Alerts;
using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.Settings;
using Chameleon.Infrastructure.Settings;
using Chameleon.Interfaces.Environment;
using Chameleon.Interfaces.Auth;
using Chameleon.Auth.Core;
using Chameleon.Infrastructure.Api;
using Chameleon.Interfaces.Api;
using Chameleon.Auth.Api;
using Chameleon.Auth.Services;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Prism.Events;
using Chameleon.Interfaces.Startup;
using Chameleon.Maui.Application.Startup;
using Chameleon.Domain.Entities;
using Chameleon.Maui.Toolkit.Models;

namespace Chameleon.App.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            MapPageViewModelRouting();

            var builder = MauiApp.CreateBuilder()
#if DEBUG                                
                                    .UseMauiCommunityToolkit()
#else
								.UseMauiCommunityToolkit(options =>
								{
									options.SetShouldSuppressExceptionsInConverters(true);
									options.SetShouldSuppressExceptionsInBehaviors(true);
									options.SetShouldSuppressExceptionsInAnimations(true);
								})
#endif
                                    .UseMauiCommunityToolkitMarkup()
                                    .UseMauiApp<App>()
                                    .ConfigureFonts(fonts =>
                                    {
                                        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                                        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                                    })
                                    .RegisterEssentials()
                                    .RegisterAppServices();

            builder.Services.AddSingleton<PopupSizeConstants>();

            RegisterPopups(builder.Services);
            RegisterViewsAndViewModels(builder.Services);

#if DEBUG
            builder.Logging.AddDebug().SetMinimumLevel(LogLevel.Trace);
#endif
            return builder.Build();
        }

        private static void MapPageViewModelRouting()
        {
            PageViewModelRouting.Instance.AddMappings(new[] {
                //lohin
                PageViewModelRouting.CreateViewModelMapping<LoginPage, LoginPageViewModel, LoginGalleryPage, LoginGalleryViewModel>(), 
                //settings
                PageViewModelRouting.CreateViewModelMapping<MainSettingsPage, MainSettingsPageViewModel, SettingsGalleryPage, SettingsGalleryViewModel>(),
            });
        }

        public static MauiAppBuilder RegisterEssentials(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<IDeviceDisplay>(DeviceDisplay.Current);
            mauiAppBuilder.Services.AddSingleton<IDeviceInfo>(DeviceInfo.Current);
            mauiAppBuilder.Services.AddSingleton<IFileSaver>(FileSaver.Default);
            mauiAppBuilder.Services.AddSingleton<IFolderPicker>(FolderPicker.Default);
            mauiAppBuilder.Services.AddSingleton<IBadge>(Badge.Default);
            return mauiAppBuilder;
        }

        public static MauiAppBuilder RegisterAppServices(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<IEventAggregator, EventAggregator>();

            mauiAppBuilder.Services.AddSingleton<IApplicationEnvironment, ApplicationEnvironment>();
            mauiAppBuilder.Services.AddSingleton<IAlertService, AlertService>();
            mauiAppBuilder.Services.AddSingleton<INavigationService, MauiNavigationService>();
            
            //settings
            mauiAppBuilder.Services.AddSingleton<ISettingsService, MauiSettingsService>();
            mauiAppBuilder.Services.AddSingleton<IApplicationSettingsService, ApplicationSettingsService>();

            //Todo: might be merged
            mauiAppBuilder.Services.AddSingleton<IApplicationConfigurationManagerService, ApplicationConfigurationManagerService>();
            mauiAppBuilder.Services.AddSingleton<IApplicationConfigurationManager, ApplicationConfigurationManager>();
            mauiAppBuilder.Services.AddSingleton<IApplicationConfiguration, ApplicationConfiguration>();
            mauiAppBuilder.Services.AddSingleton<IUrlConfiguration, UrlConfiguration>();

            //popups
            mauiAppBuilder.Services.AddSingleton<IPopupDialogService, PopupDialogManagerService>();

            //startup
            mauiAppBuilder.Services.AddSingleton<IApplicationStartup, ApplicationStartup>();

            //user
            mauiAppBuilder.Services.AddSingleton<IApplicationUser, ApplicationUser>();

            //auth
            mauiAppBuilder.Services.AddSingleton<IAuthSession, AuthSession>();
            mauiAppBuilder.Services.AddSingleton<IAuthService, AuthService>();

            //api
            mauiAppBuilder.Services.AddSingleton<IApiClient, ApiClient>();
            mauiAppBuilder.Services.AddSingleton<IAuthApiClient, AuthApiClient>();
            return mauiAppBuilder;
        }
        static void RegisterPopups(in IServiceCollection services)
        {
            // Add Auth popup
            services.AddTransientPopup<AuthView, AuthViewModel>();
        }

        static void RegisterViewsAndViewModels(in IServiceCollection services)
        {
            // Add Flyout Pages + ViewModels
            services.AddTransient<SettingsGalleryPage, SettingsGalleryViewModel>();
            services.AddTransient<LoginGalleryPage, LoginGalleryViewModel>();

            // Add Settings Pages + ViewModels
            services.AddTransientWithShellRoute<MainSettingsPage, MainSettingsPageViewModel>();

            // Add Login Pages + ViewModels
            services.AddSingletonWithShellRoute<LoginPage, LoginPageViewModel>();
        }


        static IServiceCollection AddTransientWithShellRoute<TPage, TViewModel>(this IServiceCollection services) where TPage : BasePage<TViewModel>
                                                                                                            where TViewModel : BaseViewModel
        {
            return services.AddTransientWithShellRoute<TPage, TViewModel>(PageViewModelRouting.Instance.GetPageRoute<TViewModel>());
        }

        static IServiceCollection AddSingletonWithShellRoute<TPage, TViewModel>(this IServiceCollection services) where TPage : BasePage<TViewModel>
                                                                                                       where TViewModel : BaseViewModel
        {
            return services.AddSingletonWithShellRoute<TPage, TViewModel>(PageViewModelRouting.Instance.GetPageRoute<TViewModel>());
        }
    }
}
