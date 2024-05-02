using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfileFolders;

namespace Chameleon.Application.Events
{
    public interface IUserProfileFolderEventHandler
        : ISingletonDependency
    {
        Task DeleteFolder(IUserProfileFolder userProfileFolder);
    }
}
