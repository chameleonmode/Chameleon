using Chameleon.Interfaces.YouTube;

namespace Chameleon.Avalonia.Controls.UserProfileView.Models.Youtube;

public class YoutubeSettingsBindableMapProfile : AutoMapper.Profile
{
    public YoutubeSettingsBindableMapProfile()
    {
        var map = CreateMap<YoutubeSettingsBindable, IYouTubeSettings>();

        map.ReverseMap();
    }
}