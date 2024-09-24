using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Favorites
{
    public interface IFavoritesUserProfileView 
        : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        IFavoritesUserProfile FavoritesUserProfile { get; set; }
    }
}
