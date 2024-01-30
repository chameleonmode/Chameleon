using Chameleon.Interfaces.Ioc;

namespace Chameleon.Controls.AssistantUsers.Interfaces
{
    public interface IInviteUserOrAddProfilesViewModel 
        : ITransientDependency
    {
        bool ShowInviteinfo { get; set; }

        string TitleText { get; set; }

        string SaveChangesBtnText { get; set; }

        long AssistantId { get; set; }
    }
}
