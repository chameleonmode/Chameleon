using Chameleon.Interfaces.Favorites;
using Chameleon.Interfaces.UserProfiles;
using System.Linq;

namespace Chameleon.Infrastructure.Favorites
{
    public class FavoritesService : IFavoritesService
    {
        private readonly IUserProfileService _userProfileService;
        public FavoritesService(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        private IUserProfiles _userProfiles;
        public bool IsFavoritesHasNoItems 
        {
            get => IsUserProfilesHasNoFavorites();
        }

        private bool IsUserProfilesHasNoFavorites()
        {
            //_userProfiles = _userProfileService.GetAll();
            //return !_userProfiles.Any(x => x.IsFavourite);
						throw new NotImplementedException();
        }
    }
}
