using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.Views;

namespace Chameleon.Interfaces.UserProfiles
{
    public interface IAddUserProfilesPopupView
        : IViewControl
        , ITransientDependency
    {
        //IUserProfileFolder Folder { get; set; }
    }
}
