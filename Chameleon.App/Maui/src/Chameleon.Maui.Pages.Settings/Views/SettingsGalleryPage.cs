using Chameleon.Maui.Pages.Settings.ViewModels;
using Chameleon.Maui.Toolkit.Base;

namespace Chameleon.Maui.Pages.Settings.Views;

public class SettingsGalleryPage : BaseGalleryPage<SettingsGalleryViewModel>
{
    public SettingsGalleryPage(SettingsGalleryViewModel viewsGalleryViewModel)
        : base("Settings", viewsGalleryViewModel)
    {
    }
}
