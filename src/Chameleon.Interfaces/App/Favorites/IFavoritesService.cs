using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Favorites
{
    public interface IFavoritesService
        : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        bool IsFavoritesHasNoItems { get; }
    }
}
