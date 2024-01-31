using Chameleon.Interfaces.Services;
using Chameleon.Maui.Toolkit.Base;
using Chameleon.Maui.Toolkit.Models;

namespace Chameleon.Maui.Pages.Settings.ViewModels;
public class SettingsGalleryViewModel : BaseGalleryViewModel
{
    public SettingsGalleryViewModel(INavigationService navigationService) : base(
        [
            SectionModel.Create<MainSettingsPageViewModel>("App Settings", "Edit the applications settings"),
        ], 
        navigationService)
    {
    }
}
