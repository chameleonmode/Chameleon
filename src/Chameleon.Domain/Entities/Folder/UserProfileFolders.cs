using Chameleon.Interfaces.UserProfileFolders;
using System;
using System.Linq;

namespace Chameleon.Domain.Entities
{
    public class UserProfileFolders 
        : ObservableEntities<IUserProfileFolder>
        , IUserProfileFolders
    {
        public bool HasWithTitle(string title)
        {
            return this.Any(profile => string.Equals(
                profile.Title, title, StringComparison.InvariantCultureIgnoreCase
            ));
        }
    }
}
