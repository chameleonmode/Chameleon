using Chameleon.Interfaces.Entities;

namespace Chameleon.Interfaces.Favorites
{
    public interface IFavoritesUserProfileInfo : IEntity
    {
        string Title { get; set; }
    }
}
