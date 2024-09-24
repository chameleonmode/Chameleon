using Chameleon.Interfaces.Ioc;

namespace Chameleon.Controls.AssistantUsers.Interfaces
{
    public interface IDeleteAssistantUserPopupViewModel 
        : Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
    {
        string UserName { get; set; }
    }
}
