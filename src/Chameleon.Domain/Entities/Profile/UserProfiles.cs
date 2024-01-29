using Chameleon.Interfaces.UserProfiles;
using System;
using System.Linq;

namespace Chameleon.Domain.Entities
{
    public class UserProfiles 
        : ObservableEntities<IUserProfile>
        , IUserProfiles
    {
        public bool HasWithTitle(string title)
        {
            return this.Any(profile => string.Equals(
                profile.Title, title, StringComparison.InvariantCultureIgnoreCase
            ));
        }

        public IUserProfile GetByTitle(string title)
        {
            return this.FirstOrDefault(profile => string.Equals(
                profile.Title, title, StringComparison.InvariantCultureIgnoreCase
            ));
        }
    }
}
