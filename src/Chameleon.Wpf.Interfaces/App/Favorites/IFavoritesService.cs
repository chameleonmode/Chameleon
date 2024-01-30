using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Favorites
{
    public interface IFavoritesService
        : ISingletonDependency
    {
        bool IsFavoritesHasNoItems { get; }
    }
}
