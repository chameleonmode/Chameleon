using Chameleon.Interfaces.Bookmarks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.Domain.Entities
{
    public class ProfileBookmarks
        : ObservableCollection<IProfileBookmark>
        , IProfileBookmarks
    {
        public ProfileBookmarks(
           int profileId,
           IEnumerable<IProfileBookmark> collection
           ) : base(collection)
        {
            ProfileId = profileId;
        }

        public int ProfileId { get; }
    }
}
