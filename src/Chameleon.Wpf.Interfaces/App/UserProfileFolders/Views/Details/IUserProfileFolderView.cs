using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.UserProfileFolders
{
    public interface IUserProfileFolderView 
        : ISingletonDependency
    {
        IUserProfileFolder UserProfileFolder { get; set; }
    }
}
