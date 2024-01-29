using Chameleon.Interfaces.App.Prospector;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Chameleon.Domain.Entities
{
    public class UserProfileProspectorBlogsOfInterests
        : ObservableCollection<IUserProfileProspectorBlogsOfInterest>
        , IUserProfileProspectorBlogsOfInterests
    {
        public UserProfileProspectorBlogsOfInterests(
            int profileId,
            IEnumerable<IUserProfileProspectorBlogsOfInterest> collection
            ) : base(collection)
        {
            ProfileId = profileId;
        }

        public int ProfileId { get; }
    }
}
