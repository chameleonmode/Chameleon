using Chameleon.Interfaces.Ioc;

namespace Chameleon.Controls.AssistantUsers.Interfaces
{
    public interface IInviteUserOrAddProfilesPopupService 
        : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        void ShowPopup(bool showInviteInfo, long? userAssistantId = null);
    }
}
