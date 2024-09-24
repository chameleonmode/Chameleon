using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Views;

namespace Chameleon.Interfaces.UserProfileFolders
{
    public interface IDeleteFolderPopupView 
        : IViewControl
        , Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
    {
        string FolderName { get; set; }
    }
}
