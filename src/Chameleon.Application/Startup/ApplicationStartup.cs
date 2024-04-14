using Chameleon.Application.Events;
using Chameleon.Common.Regions;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dashboard;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.Startup;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.Views;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.Interfaces.Windows;
using Chameleon.Prism.Events;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Chameleon.Application.Startup
{
    public class ApplicationStartup : IApplicationStartup
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IAuthService _authService;
        private readonly IMainWindow _mainWindow;
        private readonly ISystemBrowserManager _systemBrowserManager;

        public ApplicationStartup(
             IEventAggregator eventAggregator,
             IAuthService authService,
            // injected just to create all event handlers to start them up
            //ISettingsViewModel ___,
            //IDashboardViewModel __,
           // IProjectsView ____,
            //IUserProfilesViewModel _____,
            //IUserProfileFoldersViewModel ______,
             //IUserProfileIdentityViewModel ______,
             ISystemBrowserManager systemBrowserManager 
            ,IEnumerable<IApplicationEventHandlers> _
            )
        {
            _authService = authService;
             _eventAggregator = eventAggregator;
            //_mainWindow = mainWindow;

            _systemBrowserManager = systemBrowserManager;

            _eventAggregator
                .GetEvent<LoginCancelEvent>()
                .SubscribeOnce(CloseApplication);
        }

        public async Task RunAsync()
        {
            try
            {
                if (!await _authService.LoginAsync())
                    await _authService.ShowLoginDialogAsync();
            }                 
            catch
            {
                await _authService.ShowLoginDialogAsync();
            }
        }

        private void CloseApplication()
        {
            Environment.Exit(0);
        }

        public void Run()
        {
            _authService.Login();
        }
    }
}
