using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.UserProfileFolders
{
    public interface IUserProfileFolderView 
        : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        IUserProfileFolder UserProfileFolder { get; set; }
    }
}
