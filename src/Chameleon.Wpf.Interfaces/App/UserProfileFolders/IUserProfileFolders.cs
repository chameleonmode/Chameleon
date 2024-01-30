using System.Collections.Generic;
using System.Collections.Specialized;

namespace Chameleon.Interfaces.UserProfileFolders
{
    public interface IUserProfileFolders 
        : IReadOnlyList<IUserProfileFolder>
        , INotifyCollectionChanged
    {
        bool HasWithTitle(string title);
    }
}
