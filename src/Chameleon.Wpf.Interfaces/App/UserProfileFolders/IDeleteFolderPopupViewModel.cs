using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.UserProfileFolders
{
    public interface IDeleteFolderPopupViewModel : ITransientDependency
    {
        string FolderName { get; set; }
    }
}
