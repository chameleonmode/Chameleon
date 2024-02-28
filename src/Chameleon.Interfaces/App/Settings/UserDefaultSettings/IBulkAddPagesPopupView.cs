using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Views;

namespace Chameleon.Interfaces.UserProfileFolders
{
    public interface IBulkAddPagesPopupView
        : IViewControl
        , ITransientDependency
    {
        string Urls { set; get; }
    }
}
