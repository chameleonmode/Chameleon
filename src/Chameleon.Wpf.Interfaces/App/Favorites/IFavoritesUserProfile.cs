using Chameleon.Interfaces.UserProfiles;
using System.Collections.Generic;

namespace Chameleon.Interfaces.Favorites
{
    public interface IFavoritesUserProfile : IFavoritesUserProfileInfo
    {
        IReadOnlyList<IUserProfile> Profiles { get; }
    }
}
