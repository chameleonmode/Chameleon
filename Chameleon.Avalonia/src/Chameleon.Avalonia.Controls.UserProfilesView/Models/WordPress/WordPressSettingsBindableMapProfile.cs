using Chameleon.Interfaces.WordPress;

namespace Chameleon.Avalonia.Controls.UserProfileView.Models.WordPress;

public class WordPressSettingsBindableMapProfile : AutoMapper.Profile
{
    public WordPressSettingsBindableMapProfile()
    {
        ProxySettingsDtoMap();
    }

    private void ProxySettingsDtoMap()
    {
        var map = CreateMap<WordPressSettingsBindable, IWordPressSettings>();

        map.ReverseMap();
    }
}
