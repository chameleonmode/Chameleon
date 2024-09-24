using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Views;

namespace Chameleon.Controls.AssistantUsers.Interfaces
{
    public interface IDeleteAssistantUserPopupView 
        : IViewControl
        , Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
    {
        string UserName { get; set; }
    }
}
