using Chameleon.Application.Events;
using Chameleon.Common.Regions;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dashboard;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.Startup;
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
            IDashboardViewModel __,
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

            _eventAggregator
              .GetEvent<LoginSuccessEvent>()
              .SubscribeOnce(ShowMainWindow);
        }

        public async Task RunAsync()
        {
            //await _authService.ShowLoginDialogAsync();

           await _authService.LoginAsync();
            //if (string.IsNullOrEmpty(_settingsService.AuthAccessToken))
            //{
            // first create dialog not showing it

            //}
            // else
            // {
            //     _authManager.Login();
            // }
            //
            // return Task.CompletedTask;
        }

        private void ShowMainWindow()
        {
            //_mainWindow.SetContent(RegionNames.ContentRegion);
            //AutoUpdater.NET - check if newer version of app exists
            //AutoUpdater.Start("https://chameleonaccess.s3-us-west-2.amazonaws.com/AutoUpdater.xml");

           // _systemBrowserManager
           //       .Get(SystemBrowserType.Chrome)
           //       .Open(new SystemBrowserLaunchOptions
           //       {
           //           Url = new Uri("https://stackoverflow.com/questions/38326055/setting-network-credentials-for-simple-webrequest"),
           //           SignIn = false,
           //           UserProfile = new UserProfile() { Id = 123, FolderId = 123 }
           //       });
           //
           // _systemBrowserManager
           //  .Get(SystemBrowserType.Firefox)
           //  .Open(new SystemBrowserLaunchOptions
           //  {
           //      Url = new Uri("https://stackoverflow.com/questions/38326055/setting-network-credentials-for-simple-webrequest"),
           //      SignIn = false,
           //      UserProfile = new UserProfile() { Id = 123, FolderId = 123 }
           //  });
           //
           //_systemBrowserManager
           //.Get(SystemBrowserType.Brave)
           //.Open(new SystemBrowserLaunchOptions
           //{
           //    Url = new Uri("https://stackoverflow.com/questions/38326055/setting-network-credentials-for-simple-webrequest"),
           //    SignIn = false,
           //    UserProfile = new UserProfile() { Id = 123, FolderId = 123 }
           //});
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
