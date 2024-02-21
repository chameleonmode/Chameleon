using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.Startup;

namespace Chameleon.App.Maui
{
    public partial class App : Application
    {
        private readonly ISettingsService _settingsService;
        private readonly INavigationService _navigationService;
        private readonly IApplicationStartup _applicationStartup;
        public App(ISettingsService settingsService, INavigationService navigationService, IApplicationStartup applicationStartup)
        {
            _settingsService = settingsService;
            _navigationService = navigationService;

            InitializeComponent();

            MainPage = new AppShell(navigationService, applicationStartup);
        }
    }
}
