using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Views;

namespace Chameleon.Interfaces.UserProfileFolders
{
    public interface IDeleteFolderPopupView 
        : IViewControl
        , ITransientDependency
    {
        string FolderName { get; set; }
    }
}
