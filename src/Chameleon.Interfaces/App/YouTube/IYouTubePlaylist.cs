using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.YouTube
{
    public interface IYouTubePlaylist
        : Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
    {
        string Id { get; set; }
        string Name { get; set; }
        bool IsSelected { get; set; }
    }
}
