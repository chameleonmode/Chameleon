using Chameleon.Interfaces.Ioc;

namespace Chameleon.Controls.AssistantUsers.Interfaces
{
    public interface IDeleteAssistantUserPopupViewModel 
        : ITransientDependency
    {
        string UserName { get; set; }
    }
}
