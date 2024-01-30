using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Views;

namespace Chameleon.Controls.AssistantUsers.Interfaces
{
    public interface IDeleteAssistantUserPopupView 
        : IViewControl
        , ITransientDependency
    {
        string UserName { get; set; }
    }
}
