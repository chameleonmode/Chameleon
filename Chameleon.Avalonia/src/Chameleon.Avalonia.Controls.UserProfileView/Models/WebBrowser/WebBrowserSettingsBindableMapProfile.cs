using Chameleon.Interfaces.WebBrowser;

namespace Chameleon.Avalonia.Controls.UserProfileView.Models.WebBrowser;

public class WebBrowserSettingsBindableMapProfile : AutoMapper.Profile
{
    public WebBrowserSettingsBindableMapProfile()
    {
        ProxySettingsDtoMap();
    }

    private void ProxySettingsDtoMap()
    {
        var map = CreateMap<WebBrowserSettingsBindable, IWebBrowserSettings>();

        map.ReverseMap();
    }
}