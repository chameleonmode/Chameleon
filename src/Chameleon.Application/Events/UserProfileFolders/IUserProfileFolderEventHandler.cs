using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfileFolders;

namespace Chameleon.Application.Events
{
    public interface IUserProfileFolderEventHandler
        : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        Task DeleteFolder(IUserProfileFolder userProfileFolder);
    }
}
