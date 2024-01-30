using System.Collections.Generic;
using System.Collections.Specialized;

namespace Chameleon.Interfaces.UserProfiles
{
    public interface IUserProfiles 
        : IReadOnlyList<IUserProfile>
        , INotifyCollectionChanged
    {
        bool HasWithTitle(string title);
        IUserProfile GetByTitle(string title);
    }
}
