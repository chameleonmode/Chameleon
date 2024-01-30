using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.Views;

namespace Chameleon.Interfaces.App.UserProfiles
{
    public interface ISharingOptionsForUserProfilePopupView : IViewControl, ITransientDependency
    {
        IUserProfile UserProfile { get; set; }
    }
}
