using Chameleon.Interfaces.Ioc;

namespace Chameleon.Controls.AssistantUsers.Interfaces
{
    public interface IInviteUserOrAddProfilesPopupService 
        : ISingletonDependency
    {
        void ShowPopup(bool showInviteInfo, long? userAssistantId = null);
    }
}
