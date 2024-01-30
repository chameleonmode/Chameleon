using Chameleon.Interfaces.Ioc;
using System;

namespace Chameleon.Interfaces.UserProfiles
{
    public interface IUserProfilesView 
        : ITransientDependency
    {
        Func<IUserProfile, bool> Filter { get; set; }
        void Refresh();
    }
}
