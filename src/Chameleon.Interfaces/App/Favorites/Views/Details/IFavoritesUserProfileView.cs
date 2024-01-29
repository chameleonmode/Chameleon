using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Favorites
{
    public interface IFavoritesUserProfileView 
        : ISingletonDependency
    {
        IFavoritesUserProfile FavoritesUserProfile { get; set; }
    }
}
