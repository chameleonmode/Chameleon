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
        var map = CreateMap<ProxySettingsBindable, Chameleon.Domain.Entities.ProxySettings>();

        map.ReverseMap();
    }
}
