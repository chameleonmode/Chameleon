using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.Views;

namespace Chameleon.Interfaces.App.UserProfiles
{
    public interface ISharingOptionsForUserProfilePopupView : IViewControl, Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
    {
        IUserProfile UserProfile { get; set; }
    }
}
