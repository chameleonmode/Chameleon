using Chameleon.Maui.Pages.Login.ViewModels;
using Chameleon.Maui.Toolkit.Base;

namespace Chameleon.Maui.Pages.Login.Views;

public class LoginGalleryPage : BaseGalleryPage<LoginGalleryViewModel>
{
    public LoginGalleryPage(LoginGalleryViewModel viewsGalleryViewModel)
        : base("Login", viewsGalleryViewModel)
    {
    }
}
