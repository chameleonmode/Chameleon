using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.UserProfileFolders
{
    public interface IDeleteFolderPopupViewModel : Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
    {
        string FolderName { get; set; }
    }
}
