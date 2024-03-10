using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Avalonia.Controls.UserProfileView.Models.ProxySettings;

public class ProxySettingsBindableMapProfile : AutoMapper.Profile
{
    public ProxySettingsBindableMapProfile()
    {
        ProxySettingsDtoMap();
    }

    private void ProxySettingsDtoMap()
    {
        var map = CreateMap<ProxySettingsBindable, IProxySettings>();

        map.ReverseMap();
    }
}
