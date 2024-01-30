using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;
using System;

namespace Chameleon.Interfaces.Publishub
{
    public interface IPublishubView
    : ITransientDependency
    , IUserProfileAccessor
    {
        bool IsInitialized { get; }
    }
}
